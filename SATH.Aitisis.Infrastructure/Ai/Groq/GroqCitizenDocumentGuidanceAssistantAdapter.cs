using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SATH.Aitisis.App.Services.DocumentGuidance;

namespace SATH.Aitisis.Infrastructure.Ai.Groq
{
    public sealed class GroqCitizenDocumentGuidanceAssistantAdapter : ICitizenDocumentGuidanceAssistant
    {
        private readonly HttpClient _httpClient;
        private readonly GroqAiOptions _options;

        public GroqCitizenDocumentGuidanceAssistantAdapter(HttpClient httpClient, IOptions<GroqAiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }



        public async Task<DocumentGuidanceResult> GenerateGuidanceAsync(DocumentGuidanceRequest request)
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                return DocumentGuidanceResult.Failure("Groq API key is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_options.Model))
            {
                return DocumentGuidanceResult.Failure("Groq model is not configured.");
            }

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
                    var errorContent =await response.Content.ReadAsStringAsync();

                    return DocumentGuidanceResult.Failure($"Groq request failed with status {(int)response.StatusCode}: {errorContent}");
                }

                var groqResponse = await response.Content.ReadFromJsonAsync<GroqChatCompletionResponse>();
                var content = groqResponse?.Choices?.FirstOrDefault()?.Message?.Content;

                if (string.IsNullOrWhiteSpace(content))
                {
                    return DocumentGuidanceResult.Failure("Groq returned an empty response.");
                }

                var structuredResult =
                    JsonSerializer.Deserialize<GroqDocumentGuidanceResponse>(
                        content,
                        JsonOptions());

                if (structuredResult == null)
                {
                    return DocumentGuidanceResult.Failure("Groq response could not be parsed.");
                }

                var items = structuredResult.Documents?
                    .Select(x => new DocumentGuidanceItem
                    {
                        DocumentName = x.DocumentName ?? string.Empty,
                        Steps = x.Steps ?? Array.Empty<string>()
                    })
                    .Where(x => !string.IsNullOrWhiteSpace(x.DocumentName))
                    .ToArray()
                    ?? Array.Empty<DocumentGuidanceItem>();

                return DocumentGuidanceResult.Success(
                    structuredResult.Summary ?? string.Empty,
                    items,
                    structuredResult.GeneralAdvice ?? string.Empty,
                    string.IsNullOrWhiteSpace(structuredResult.Disclaimer)
                        ? DefaultDisclaimer()
                        : structuredResult.Disclaimer);
            }
            catch (JsonException)
            {
                return DocumentGuidanceResult.Failure("Groq response was not valid structured JSON.");
            }
            catch (HttpRequestException)
            {
                return DocumentGuidanceResult.Failure("Groq request could not be completed.");
            }
            catch (TaskCanceledException)
            {
                return DocumentGuidanceResult.Failure("Groq request timed out.");
            }
        }

        private object BuildPayload(DocumentGuidanceRequest request)
        {
            var inputJson = JsonSerializer.Serialize(
                new
                {
                    applicationId = request.ApplicationId,
                    applicationType = request.ApplicationTypeName,
                    documents = request.Documents
                        .OrderBy(x => x.DisplayOrder)
                        .Select(x => new
                        {
                            name = x.Name,
                            description = x.Description,
                            isRequired = x.IsRequired
                        })
                },
                JsonOptions());

            return new
            {
                model = _options.Model,
                temperature = 0.2,
                max_tokens = 1000,
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
                            "Prepare citizen-facing guidance for obtaining the required documents. " +
                            "Use only the provided application type and document requirement names/descriptions.\n\n" +
                            inputJson
                    }
                },
                response_format = new
                {
                    type = "json_schema",
                    json_schema = new
                    {
                        name = "citizen_document_guidance",
                        schema = new
                        {
                            type = "object",
                            additionalProperties = false,
                            required = new[]
                            {
                                "summary",
                                "documents",
                                "generalAdvice",
                                "disclaimer"
                            },
                            properties = new
                            {
                                summary = new
                                {
                                    type = "string"
                                },
                                documents = new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        additionalProperties = false,
                                        required = new[]
                                        {
                                            "documentName",
                                            "steps"
                                        },
                                        properties = new
                                        {
                                            documentName = new
                                            {
                                                type = "string"
                                            },
                                            steps = new
                                            {
                                                type = "array",
                                                items = new
                                                {
                                                    type = "string"
                                                }
                                            }
                                        }
                                    }
                                },
                                generalAdvice = new
                                {
                                    type = "string"
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


        private static string SystemPrompt()
        {
            return
                "You are an AI assistant helping a citizen prepare required PDF documents for an application. " +
                "Use only the application type and document requirement names/descriptions provided by the system. " +
                "You communicate with a greek Citizen. Speak in English please. " +
                "Do not provide guaranteed official legal or administrative instructions. " +
                "For each document, provide 1-3 practical steps explaining where the citizen may usually obtain it, " +
                "such as an official public service portal, municipality, tax authority, social insurance service, " +
                "or another competent authority, depending on the document name. " +
                "Return compact JSON only. Do not include markdown.";
        }


        private static string DefaultDisclaimer()
        {
            return "This AI guidance is informational only. Always verify document requirements and issuing procedures with the official competent authority.";
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


        private sealed class GroqDocumentGuidanceResponse
        {
            [JsonPropertyName("summary")]
            public string? Summary { get; set; }

            [JsonPropertyName("documents")]
            public GroqDocumentGuidanceItem[]? Documents { get; set; }

            [JsonPropertyName("generalAdvice")]
            public string? GeneralAdvice { get; set; }

            [JsonPropertyName("disclaimer")]
            public string? Disclaimer { get; set; }
        }


        private sealed class GroqDocumentGuidanceItem
        {
            [JsonPropertyName("documentName")]
            public string? DocumentName { get; set; }

            [JsonPropertyName("steps")]
            public string[]? Steps { get; set; }
        }
    }
}