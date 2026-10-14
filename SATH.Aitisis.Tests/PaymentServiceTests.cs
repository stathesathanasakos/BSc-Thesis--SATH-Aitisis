using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SATH.Aitisis.App.Services.Payments;
using SATH.Aitisis.Domain.Entities;
using SATH.Aitisis.Domain.Enums;
using Xunit;

namespace SATH.Aitisis.Tests;

public class PaymentServiceTests
{
    [Fact]
    public async Task StartPaymentAsync_MissingRequiredDocument_ReturnsFailure()
    {
        var repository = new Mock<IPaymentRepository>();
        var gateway = new Mock<IPaymentGateway>();

        var application = new Application
        {
            Id = 1,
            CitizenId = "citizen-1",
            Status = ApplicationStatus.Draft,
            ApplicationType = new ApplicationType
            {
                RequiresPayment = true,
                FeeAmount = 20m,
                DocumentRequirements = new List<DocumentRequirement>
                {
                    new DocumentRequirement
                    {
                        Id = 10,
                        Name = "Identity document",
                        IsRequired = true
                    }
                }
            }
        };

        // No documents have been uploaded
        repository
            .Setup(r => r.GetApplicationForPaymentAsync(1, "citizen-1"))
            .ReturnsAsync(application);

        var service = new PaymentService(
            repository.Object,
            gateway.Object,
            NullLogger<PaymentService>.Instance
        );

        var request = new StartPaymentRequest
        {
            ApplicationId = 1,
            CitizenId = "citizen-1",
            SuccessUrl = "https://example.com/success",
            CancelUrl = "https://example.com/cancel"
        };

        var result = await service.StartPaymentAsync(request);

        // Assert that the result indicates failure and that no payment was created
        Assert.False(result.Succeeded);
        Assert.Equal("Please upload all required documents before starting payment.", result.ErrorMessage);

        repository.Verify(r => r.AddPaymentAsync(It.IsAny<Payment>()), Times.Never);
        repository.Verify(r => r.SaveChangesAsync(), Times.Never);
        gateway.Verify(g => g.CreateCheckoutSessionAsync(It.IsAny<PaymentCheckoutRequest>()), Times.Never);
    }


    [Fact]
    public async Task CompletePaymentAsync_DifferentCitizen_ReturnsFailure()
    {
        var repository = new Mock<IPaymentRepository>();
        var gateway = new Mock<IPaymentGateway>();

        var application = new Application
        {
            Id = 1,
            CitizenId = "citizen-owner",
            Status = ApplicationStatus.Draft
        };

        var payment = new Payment
        {
            Id = 5,
            ApplicationId = application.Id,
            Application = application,
            Status = PaymentStatus.Pending,
            ExternalPaymentId = "session-1"
        };

        repository
            .Setup(r => r.GetPaymentByExternalSessionIdForUpdateAsync("session-1"))
            .ReturnsAsync(payment);

        var service = new PaymentService(
            repository.Object,
            gateway.Object,
            NullLogger<PaymentService>.Instance
        );

        var request = new CompletePaymentRequest
        {
            ExternalSessionId = "session-1",
            CitizenId = "citizen-other"
        };

        var result = await service.CompletePaymentAsync(request);

        // Assert that the result indicates failure and that the payment status remains unchanged
        Assert.False(result.Succeeded);
        Assert.Equal("The payment does not belong to the current user.", result.ErrorMessage);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Equal(ApplicationStatus.Draft, application.Status);

        repository.Verify(r => r.SaveChangesAsync(), Times.Never);

        gateway.Verify(g => g.GetSessionStatusAsync(It.IsAny<string>()), Times.Never);
    }


    [Fact]
    public async Task CompletePaymentAsync_UnpaidSession_ReturnsFailure()
    {
        // Arrange the mocks and set up the test data
        var repository = new Mock<IPaymentRepository>();
        var gateway = new Mock<IPaymentGateway>();

        var application = new Application
        {
            Id = 1,
            CitizenId = "citizen-1",
            Status = ApplicationStatus.Draft
        };

        var payment = new Payment
        {
            Id = 5,
            ApplicationId = application.Id,
            Application = application,
            Status = PaymentStatus.Pending,
            ExternalPaymentId = "session-1"
        };

        repository
            .Setup(r => r.GetPaymentByExternalSessionIdForUpdateAsync("session-1"))
            .ReturnsAsync(payment);

        gateway
            .Setup(g => g.GetSessionStatusAsync("session-1"))
            .ReturnsAsync(PaymentGatewaySessionStatus.Unpaid("session-1", "unpaid"));

        repository
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new PaymentService(repository.Object, gateway.Object, NullLogger<PaymentService>.Instance);

        var request = new CompletePaymentRequest
        {
            ExternalSessionId = "session-1",
            CitizenId = "citizen-1"
        };

        var result = await service.CompletePaymentAsync(request);

        // Assert that the result indicates failure and that the payment status is updated to Failed
        Assert.False(result.Succeeded);
        Assert.Equal("The payment was not completed.", result.ErrorMessage);
        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.Null(payment.PaymentDate);
        Assert.Equal(ApplicationStatus.Draft, application.Status);

        gateway.Verify(g => g.GetSessionStatusAsync("session-1"), Times.Once);

        repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}