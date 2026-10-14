using SATH.Aitisis.App.Services.Applications.StatusTransitions;
using SATH.Aitisis.Domain.Enums;
using Xunit;

namespace SATH.Aitisis.Tests;

public class ApplicationStatusTransitionTests
{
    [Fact]
    public void CanApprove_SubmittedApplication_ReturnsTrue()
    {
        // Arrange: prepare the object to test.
        var policy = new ApplicationStatusTransitionPolicy();
        // Act: execute the operation.
        var result = policy.CanApprove(ApplicationStatus.Submitted);

        // Assert: verify the expected result.
        Assert.True(result);
    }

    [Fact]
    public void CanReject_SubmittedApplication_ReturnsTrue()
    {
        var policy = new ApplicationStatusTransitionPolicy();
        var result = policy.CanReject(ApplicationStatus.Submitted);

        Assert.True(result);
    }

    [Fact]
    public void CanApprove_DraftApplication_ReturnsFalse()
    {
        var policy = new ApplicationStatusTransitionPolicy();
        var result = policy.CanApprove(ApplicationStatus.Draft);

        Assert.False(result);
    }

    [Fact]
    public void CanReject_DraftApplication_ReturnsFalse()
    {
        var policy = new ApplicationStatusTransitionPolicy();
        var result = policy.CanReject(ApplicationStatus.Draft);

        Assert.False(result);
    }

    [Fact]
    public void CanTransition_ApprovedToRejected_ReturnsFalse()
    {
        var policy = new ApplicationStatusTransitionPolicy();
        var result = policy.CanTransition(ApplicationStatus.Approved, ApplicationStatus.Rejected);

        Assert.False(result);
    }

    [Fact]
    public void CanTransition_RejectedToApproved_ReturnsFalse()
    {
        var policy = new ApplicationStatusTransitionPolicy();
        var result = policy.CanTransition(ApplicationStatus.Rejected, ApplicationStatus.Approved);

        Assert.False(result);
    }
}