using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SATH.Aitisis.App.Services.AiReview;
using SATH.Aitisis.App.Services.Applications;
using Xunit;

using EmployeeApplicationsController = SATH.Aitisis.Web.Areas.Employee.Controllers.ApplicationsController;

namespace SATH.Aitisis.Tests;

public class EmployeeApplicationsControllerTests
{
    [Fact]
    public async Task Approve_ServiceFailure_SetsErrorAndRedirectsToIndexWithoutId()
    {
        // to test the Approve action when the service returns a failure result
        // I set up the mock service to return a failure result and then verify that the controller sets the error message
        // in TempData and redirects to the Index action without an id parameter
        var service = new Mock<IApplicationService>();
        const string errorMessage = "The application cannot be approved.";

        service
            .Setup(s => s.ApproveApplicationAsync(It.IsAny<ApproveApplicationRequest>()))
            .ReturnsAsync(ApplicationDecisionResult.Failure(errorMessage));

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "employee-1")
                },
                "TestAuthentication"
            )
        );

        var httpContext = new DefaultHttpContext
        {
            User = user
        };

        var controller = new EmployeeApplicationsController(service.Object, Mock.Of<IApplicationAiReviewService>())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            },
            TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        };

        var result = await controller.Approve(1, "Review completed.");

        // to verify that the result is a RedirectToActionResult and that it redirects to the Index action without an id parameter
        var redirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("Index", redirect.ActionName);
        Assert.False(redirect.RouteValues?.ContainsKey("id") ?? false);
        Assert.Equal(errorMessage, controller.TempData["ErrorMessage"]);
        Assert.False(controller.TempData.ContainsKey("SuccessMessage"));

        service.Verify(
            s => s.ApproveApplicationAsync(
                It.Is<ApproveApplicationRequest>(request =>
                    request.ApplicationId == 1 &&
                    request.EmployeeId == "employee-1" &&
                    request.EmployeeMessage == "Review completed.")
                ),
            Times.Once
        );
    }
}
