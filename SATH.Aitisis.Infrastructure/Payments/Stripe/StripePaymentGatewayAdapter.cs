using Microsoft.Extensions.Options;
using SATH.Aitisis.App.Services.Payments;
using Stripe;
using Stripe.Checkout;


namespace SATH.Aitisis.Infrastructure.Payments.Stripe
{
    public sealed class StripePaymentGatewayAdapter : IPaymentGateway
    {
        private readonly StripePaymentOptions _options;

        public StripePaymentGatewayAdapter(IOptions<StripePaymentOptions> options)
        {
            _options = options.Value;
        }

        public async Task<PaymentCheckoutResult> CreateCheckoutSessionAsync(PaymentCheckoutRequest request)
        {
            if (string.IsNullOrWhiteSpace(_options.SecretKey))
            {
                return PaymentCheckoutResult.Failure("Stripe secret key is not configured.");
            }

            if (request.Amount <= 0)
            {
                return PaymentCheckoutResult.Failure("Payment amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(request.SuccessUrl) || string.IsNullOrWhiteSpace(request.CancelUrl))
            {
                return PaymentCheckoutResult.Failure("Payment redirect URLs are not configured.");
            }

            StripeConfiguration.ApiKey = _options.SecretKey;
            var amountInCents = ConvertAmountToMinorUnits(request.Amount);

            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = request.SuccessUrl,
                CancelUrl = request.CancelUrl,
                ClientReferenceId = request.ApplicationId.ToString(),
                Metadata = new Dictionary<string, string>
                {
                    ["ApplicationId"] = request.ApplicationId.ToString(),
                    ["PaymentId"] = request.PaymentId.ToString(),
                    ["CitizenId"] = request.CitizenId
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Quantity = 1,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = GetCurrency(request.Currency),
                            UnitAmount = amountInCents,
                            ProductData =
                                new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = request.Description
                                }
                        }
                    }
                }
            };

            try
            {
                var service = new SessionService();
                var session = await service.CreateAsync(options);

                if (string.IsNullOrWhiteSpace(session.Id) || string.IsNullOrWhiteSpace(session.Url))
                {
                    return PaymentCheckoutResult.Failure("Stripe did not return a valid checkout session.");
                }

                return PaymentCheckoutResult.Success(session.Id, session.Url);
            }
            catch (StripeException ex)
            {
                return PaymentCheckoutResult.Failure(ex.StripeError?.Message ?? ex.Message);
            }
        }

        public async Task<PaymentGatewaySessionStatus> GetSessionStatusAsync(string externalSessionId)
        {
            if (string.IsNullOrWhiteSpace(_options.SecretKey))
            {
                return PaymentGatewaySessionStatus.NotFound();
            }

            if (string.IsNullOrWhiteSpace(externalSessionId))
            {
                return PaymentGatewaySessionStatus.NotFound();
            }

            StripeConfiguration.ApiKey = _options.SecretKey;

            try
            {
                var service = new SessionService();
                var session = await service.GetAsync(externalSessionId);

                if (session == null || string.IsNullOrWhiteSpace(session.Id))
                {
                    return PaymentGatewaySessionStatus.NotFound();
                }

                if (session.PaymentStatus == "paid")
                {
                    return PaymentGatewaySessionStatus.Paid(session.Id, session.PaymentIntentId, session.PaymentStatus);
                }

                return PaymentGatewaySessionStatus.Unpaid(session.Id, session.PaymentStatus);
            }
            catch (StripeException)
            {
                return PaymentGatewaySessionStatus.NotFound();
            }
        }

        private static long ConvertAmountToMinorUnits(decimal amount)
        {
            return (long)Math.Round(amount * 100, MidpointRounding.AwayFromZero);
        }

        private string GetCurrency(string requestCurrency)
        {
            if (!string.IsNullOrWhiteSpace(requestCurrency))
            {
                return requestCurrency.ToLowerInvariant();
            }

            if (!string.IsNullOrWhiteSpace(_options.Currency))
            {
                return _options.Currency.ToLowerInvariant();
            }

            return "eur";
        }
    }
}