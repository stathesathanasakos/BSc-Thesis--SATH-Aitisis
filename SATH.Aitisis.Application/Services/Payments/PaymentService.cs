using SATH.Aitisis.Domain.Entities;
using SATH.Aitisis.Domain.Enums;
using Microsoft.Extensions.Logging;


namespace SATH.Aitisis.App.Services.Payments
{
    public sealed class PaymentService : IPaymentService
    {
        private const string PaymentProvider = "Stripe";
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentGateway _paymentGateway;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IPaymentRepository paymentRepository, IPaymentGateway paymentGateway, ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _paymentGateway = paymentGateway;
            _logger = logger;
        }


        public async Task<PaymentStartResult> StartPaymentAsync(StartPaymentRequest request)
        {
            if (request.ApplicationId <= 0 ||
                string.IsNullOrWhiteSpace(request.CitizenId) ||
                string.IsNullOrWhiteSpace(request.SuccessUrl) ||
                string.IsNullOrWhiteSpace(request.CancelUrl))
            {
                return PaymentStartResult.Failure("Invalid payment request.");
            }

            var application = await _paymentRepository.GetApplicationForPaymentAsync(request.ApplicationId, request.CitizenId);

            if (application == null)
            {
                return PaymentStartResult.Failure("The application was not found.");
            }

            if (application.Status != ApplicationStatus.Draft)
            {
                return PaymentStartResult.Failure("Payment can only be started for applications in progress.");
            }

            if (!RequiredDocumentsCompleted(application))
            {
                return PaymentStartResult.Failure("Please upload all required documents before starting payment.");
            }

            if (!application.ApplicationType.RequiresPayment)
            {
                return PaymentStartResult.Failure("This application does not require payment.");
            }

            if (!application.ApplicationType.FeeAmount.HasValue ||
                application.ApplicationType.FeeAmount.Value <= 0)
            {
                return PaymentStartResult.Failure("The application fee is not configured correctly.");
            }

            var amount = application.ApplicationType.FeeAmount.Value;

            // Log the payment start attempt with relevant details
            _logger.LogInformation("Starting payment checkout for application {ApplicationId}. Amount: {Amount} {Currency}.", application.Id, amount, "eur");

            var payment = await _paymentRepository.GetPaymentByApplicationIdAsync(application.Id);

            if (payment != null && payment.Status == PaymentStatus.Completed)
            {
                return PaymentStartResult.Failure("The payment has already been completed.");
            }

            if (payment == null)
            {
                payment = new Payment
                {
                    ApplicationId = application.Id,
                    Amount = amount,
                    Status = PaymentStatus.Pending,
                    Provider = PaymentProvider,
                    CreatedAt = DateTime.UtcNow
                };

                await _paymentRepository.AddPaymentAsync(payment);
                await _paymentRepository.SaveChangesAsync();
            }
            else
            {
                payment.Amount = amount;
                payment.Status = PaymentStatus.Pending;
                payment.Provider = PaymentProvider;
                payment.PaymentDate = null;

                await _paymentRepository.SaveChangesAsync();
            }

            var checkoutRequest = new PaymentCheckoutRequest
            {
                ApplicationId = application.Id,
                PaymentId = payment.Id,
                CitizenId = request.CitizenId,
                Amount = amount,
                Currency = "eur",
                Description = $"Application fee for {application.ApplicationType.Name}",
                SuccessUrl = request.SuccessUrl,
                CancelUrl = request.CancelUrl
            };

            var checkoutResult = await _paymentGateway.CreateCheckoutSessionAsync(checkoutRequest);

            if (!checkoutResult.Succeeded || string.IsNullOrWhiteSpace(checkoutResult.ExternalSessionId) || string.IsNullOrWhiteSpace(checkoutResult.CheckoutUrl))
            {
                payment.Status = PaymentStatus.Failed;
                await _paymentRepository.SaveChangesAsync();

                // Log the failure of the payment checkout session creation with relevant details
                _logger.LogWarning("Payment checkout session creation failed for application {ApplicationId}.", application.Id);
                
                return PaymentStartResult.Failure(checkoutResult.ErrorMessage ?? "The payment session could not be created.");
            }

            payment.ExternalPaymentId = checkoutResult.ExternalSessionId;
            payment.Status = PaymentStatus.Pending;
            payment.PaymentDate = null;

            await _paymentRepository.SaveChangesAsync();

            // Log the successful creation of the payment checkout session with relevant details
            _logger.LogInformation("Payment checkout session created for application {ApplicationId} and payment {PaymentId}.", application.Id, payment.Id);

            return PaymentStartResult.Success(checkoutResult.CheckoutUrl);
        }



        public async Task<PaymentCompletionResult> CompletePaymentAsync(CompletePaymentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ExternalSessionId) || string.IsNullOrWhiteSpace(request.CitizenId))
            {
                return PaymentCompletionResult.Failure("Invalid payment completion request.");
            }

            var payment = await _paymentRepository.GetPaymentByExternalSessionIdForUpdateAsync(request.ExternalSessionId);

            if (payment == null)
            {
                return PaymentCompletionResult.Failure("The payment was not found.");
            }

            if (payment.Application.CitizenId != request.CitizenId)
            {
                return PaymentCompletionResult.Failure("The payment does not belong to the current user.");
            }

            if (payment.Status == PaymentStatus.Completed)
            {
                if (payment.Application.Status == ApplicationStatus.Draft)
                {
                    payment.Application.Status = ApplicationStatus.Submitted;
                    payment.Application.SubmissionDate = DateTime.UtcNow;
                    await _paymentRepository.SaveChangesAsync();
                }

                return PaymentCompletionResult.Success(payment.ApplicationId);
            }

            var gatewayStatus = await _paymentGateway.GetSessionStatusAsync(request.ExternalSessionId);

            if (!gatewayStatus.Exists)
            {
                payment.Status = PaymentStatus.Failed;
                await _paymentRepository.SaveChangesAsync();

                // Log a warning indicating that the external payment session was not found for the given payment
                _logger.LogWarning("External payment session was not found for payment {PaymentId}.", payment.Id);

                return PaymentCompletionResult.Failure("The external payment session was not found.");
            }

            if (!gatewayStatus.IsPaid)
            {
                payment.Status = PaymentStatus.Failed;
                await _paymentRepository.SaveChangesAsync();

                // Log a warning indicating that the payment was not completed for the given application
                _logger.LogWarning("Payment was not completed for application {ApplicationId}.", payment.ApplicationId);

                return PaymentCompletionResult.Failure("The payment was not completed.");
            }

            payment.Status = PaymentStatus.Completed;
            payment.PaymentDate = DateTime.UtcNow;

            if (payment.Application.Status == ApplicationStatus.Draft)
            {
                payment.Application.Status = ApplicationStatus.Submitted;
                payment.Application.SubmissionDate = DateTime.UtcNow;
            }

            await _paymentRepository.SaveChangesAsync();

            // Log the successful completion of the payment with relevant details
            _logger.LogInformation("Payment completed for application {ApplicationId}.", payment.ApplicationId);

            return PaymentCompletionResult.Success(payment.ApplicationId);
        }

        public async Task<PaymentDetails?> GetPaymentDetailsAsync(int applicationId, string citizenId)
        {
            if (applicationId <= 0 ||
                string.IsNullOrWhiteSpace(citizenId))
            {
                return null;
            }

            var application = await _paymentRepository.GetApplicationForPaymentAsync(applicationId, citizenId);

            if (application == null)
            {
                return null;
            }

            var requiresPayment = application.ApplicationType.RequiresPayment;

            var amount = requiresPayment && application.ApplicationType.FeeAmount.HasValue
                    ? application.ApplicationType.FeeAmount.Value
                    : 0m;

            var paymentStatus = application.Payment?.Status;

            var canStartPayment = requiresPayment && amount > 0 && paymentStatus != PaymentStatus.Completed;

            return new PaymentDetails
            {
                ApplicationId = application.Id,
                ApplicationTypeName = application.ApplicationType.Name,
                Amount = amount,
                Currency = "eur",
                RequiresPayment = requiresPayment,
                PaymentStatus = paymentStatus,
                CanStartPayment = canStartPayment
            };
        }



        private static bool RequiredDocumentsCompleted(Application application)
        {
            var requiredRequirementIds = application.ApplicationType
                .DocumentRequirements
                .Where(x => x.IsRequired)
                .Select(x => x.Id)
                .ToHashSet();

            if (requiredRequirementIds.Count == 0)
            {
                return true;
            }

            var uploadedRequirementIds = application.Documents
                .Where(x => x.DocumentRequirementId.HasValue)
                .Select(x => x.DocumentRequirementId!.Value)
                .ToHashSet();

            return requiredRequirementIds.All(uploadedRequirementIds.Contains);
        }
    }
}