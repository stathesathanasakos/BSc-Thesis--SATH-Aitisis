using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.App.Services.Applications.StatusTransitions
{
    public interface IApplicationStatusTransitionPolicy
    {
        bool CanApprove(ApplicationStatus currentStatus);

        bool CanReject(ApplicationStatus currentStatus);

        bool CanTransition(ApplicationStatus currentStatus, ApplicationStatus targetStatus);
    }
}