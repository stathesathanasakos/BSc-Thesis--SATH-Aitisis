using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SATH.Aitisis.App.Services.AiReview;
using Microsoft.Extensions.Logging;



namespace SATH.Aitisis.Infrastructure.Ai.Groq
{
    public sealed class GroqAiReviewAssistantAdapter : IAiReviewAssistant
    {
        private readonly HttpClient _httpClient;
        private readonly GroqAiOptions _options;
        private readonly ILogger<GroqAiReviewAssistantAdapter> _logger;

        public GroqAiReviewAssistantAdapter(HttpClient httpClient, IOptions<GroqAiOptions> options, ILogger<GroqAiReviewAssistantAdapter> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<AiReviewResult> GenerateReviewAsync(AiReviewRequest request)
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                return AiReviewResult.Failure("Groq API key is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_options.Model))
            {
                return AiReviewResult.Failure("Groq model is not configured.");
            }

            // Log the start of the AI review generation process
            _logger.LogInformation("Starting AI review generation for application {ApplicationId}.", request.ApplicationId);

            // I create the endpoint URL for the Groq API request
            var endpoint = $"{_options.BaseUrl.TrimEnd('/')}/chat/completions";
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
            var payload = BuildPayload(request);
            httpRequest.Content = JsonContent.Create(payload);

            try
            {
                using var response = await _httpClient.SendAsync(httpRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return AiReviewResult.Failure($"Groq request failed with status {(int)response.StatusCode}: {errorContent}");
                }

                var groqResponse = await response.Content.ReadFromJsonAsync<GroqChatCompletionResponse>();
                var content = groqResponse?.Choices?.FirstOrDefault()?.Message?.Content;

                if (string.IsNullOrWhiteSpace(content))
                {
                    // Log a warning if the Groq response is empty
                    _logger.LogWarning("Groq AI review returned empty content for application {ApplicationId}.", request.ApplicationId);
                    
                    return AiReviewResult.Failure("Groq returned an empty response.");
                }

                var structuredResult = JsonSerializer.Deserialize<GroqAiStructuredReview>(content, JsonOptions());

                if (structuredResult == null)
                {
                    // Log a warning if the Groq response could not be parsed into the expected structured format
                    _logger.LogWarning("Groq AI review response could not be parsed for application {ApplicationId}.", request.ApplicationId);

                    return AiReviewResult.Failure("Groq response could not be parsed.");
                }

                // Log the successful completion of the AI review generation process
                _logger.LogInformation("AI review generation completed for application {ApplicationId}.", request.ApplicationId);

                return AiReviewResult.Success(
                    structuredResult.Summary,
                    structuredResult.KeyInformation ?? Array.Empty<string>(),
                    structuredResult.PdfReviewFocus ?? Array.Empty<string>(),
                    string.IsNullOrWhiteSpace(structuredResult.Disclaimer)
                        ? DefaultDisclaimer()
                        : structuredResult.Disclaimer);
            }
            catch (JsonException)
            {
                return AiReviewResult.Failure("Groq response was not valid structured JSON.");
            }
            catch (HttpRequestException)
            {
                return AiReviewResult.Failure("Groq request could not be completed.");
            }
            catch (TaskCanceledException)
            {
                return AiReviewResult.Failure("Groq request timed out.");
            }
        }



        // define the payload message for the Groq API request
        private object BuildPayload(AiReviewRequest request)
        {
            var inputJson = JsonSerializer.Serialize(
                new
                {
                    applicationId = request.ApplicationId,
                    applicationType = request.ApplicationTypeName,
                    fields = request.Fields
                        .OrderBy(x => x.DisplayOrder)
                        .Select(x => new
                        {
                            label = x.Label,
                            value = x.Value
                        })
                },
                JsonOptions());

            return new
            {
                model = _options.Model,
                temperature = 0.2,
                max_tokens = 920,
                messages = new object[]
                {
                    new
                    {
                        role = "system",
                        content = SystemPrompt()
                    },
                    new
                    {
                        role = "user",
                        content =
                            "Review the following citizen application data. " +
                            "Use only the provided JSON data.\n\n" +
                            inputJson
                    }
                },
                response_format = new
                {
                    type = "json_schema",
                    json_schema = new
                    {
                        name = "employee_application_review_summary",
                        schema = new
                        {
                            type = "object",
                            additionalProperties = false,
                            required = new[]
                            {
                                "summary",
                                "keyInformation",
                                "pdfReviewFocus",
                                "disclaimer"
                            },
                            properties = new
                            {
                                summary = new
                                {
                                    type = "string"
                                },
                                keyInformation = new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "string"
                                    }
                                },
                                pdfReviewFocus = new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "string"
                                    }
                                },
                                disclaimer = new
                                {
                                    type = "string"
                                }
                            }
                        }
                    }
                }
            };
        }


        // and this is the prompt 
        private static string SystemPrompt()
        {
            return
                "You are an AI assistant helping an employee review a citizen application. " +
                "Use only the application type and submitted field values provided by the system. " +
                "Do not decide whether the application should be approved. " +
                "Do not infer facts that are not present in the data. " +
                "You can't read pdf files and other documents. " +
                "Return a concise summary, key information, and 3-4 practical PDF review focus points. " +
                "The PDF review focus should tell the employee what evidence to compare against the submitted fields.";
        }

        private static string DefaultDisclaimer()
        {
            return "This AI-generated review summary is an assistant note only and does not replace manual employee review!";
        }

        private static JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }


        private sealed class GroqChatCompletionResponse
        {
            [JsonPropertyName("choices")]
            public List<GroqChoice>? Choices { get; set; }
        }

        private sealed class GroqChoice
        {
            [JsonPropertyName("message")]
            public GroqMessage? Message { get; set; }
        }

        private sealed class GroqMessage
        {
            [JsonPropertyName("content")]
            public string? Content { get; set; }
        }

        private sealed class GroqAiStructuredReview
        {
            [JsonPropertyName("summary")]
            public string Summary { get; set; } = string.Empty;

            [JsonPropertyName("keyInformation")]
            public string[]? KeyInformation { get; set; }

            [JsonPropertyName("pdfReviewFocus")]
            public string[]? PdfReviewFocus { get; set; }

            [JsonPropertyName("disclaimer")]
            public string Disclaimer { get; set; } = string.Empty;
        }
    }
}
