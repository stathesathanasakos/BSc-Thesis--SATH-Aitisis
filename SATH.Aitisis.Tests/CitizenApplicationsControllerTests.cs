using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SATH.Aitisis.App.Services.Applications;
using SATH.Aitisis.App.Services.ApprovalCertificates;
using SATH.Aitisis.App.Services.DocumentGuidance;
using SATH.Aitisis.App.Services.Payments;
using Xunit;

using CitizenApplicationsController = SATH.Aitisis.Web.Areas.Citizen.Controllers.ApplicationsController;

namespace SATH.Aitisis.Tests;

public class CitizenApplicationsControllerTests
{
    [Fact]
    public async Task Documents_MissingCitizenId_ReturnsUnauthorizedWithoutServiceCall()
    {
        // I am testing the Documents action of the CitizenApplicationsController to ensure that it returns
        // UnauthorizedResult when the authenticated user does not have a NameIdentifier claim
        // and that the service method is never called
        var service = new Mock<IApplicationService>();

        // An authenticated identity without a NameIdentifier claim.
        var user = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), "TestAuthentication"));
        var controller = CreateController(service, user);

        // Act for the Documents action with a sample applicationId
        var result = await controller.Documents(1);

        // Assert that the result is UnauthorizedResult and that the service method was never called
        Assert.IsType<UnauthorizedResult>(result);

        service.Verify(s => s.GetApplicationDocumentsInfoAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Documents_ApplicationNotFound_ReturnsNotFound()
    {
        var service = new Mock<IApplicationService>();

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "citizen-1")
                },
                "TestAuthentication")
            );

        service
            .Setup(s => s.GetApplicationDocumentsInfoAsync(1, "citizen-1"))
            .ReturnsAsync((ApplicationDocumentsInfo?)null);

        var controller = CreateController(service, user);

        // Act for the Documents action with a sample applicationId
        var result = await controller.Documents(1);
        // Assert that the result is NotFoundResult and that the service method was called once with the expected parameters
        Assert.IsType<NotFoundResult>(result);

        service.Verify(s => s.GetApplicationDocumentsInfoAsync(1, "citizen-1"), Times.Once);
    }

    private static CitizenApplicationsController CreateController(Mock<IApplicationService> service, ClaimsPrincipal user)
    {
        var controller = new CitizenApplicationsController(
            service.Object,
            Mock.Of<IApprovalCertificateService>(),
            Mock.Of<IPaymentService>(),
            Mock.Of<IApplicationDocumentGuidanceService>()
        );

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };

        return controller;
    }
}