using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.App.Services.Applications.StatusTransitions
{
    public sealed class ApplicationStatusTransitionPolicy : IApplicationStatusTransitionPolicy
    {
        public bool CanApprove(ApplicationStatus currentStatus)
        {
            return CanTransition(currentStatus, ApplicationStatus.Approved);
        }

        public bool CanReject(ApplicationStatus currentStatus)
        {
            return CanTransition(currentStatus, ApplicationStatus.Rejected);
        }

        public bool CanTransition(ApplicationStatus currentStatus, ApplicationStatus targetStatus)
        {
            return currentStatus switch
            {
                ApplicationStatus.Submitted
                    when targetStatus == ApplicationStatus.Approved ||
                         targetStatus == ApplicationStatus.Rejected
                    => true,

                _ => false
            };
        }
    }
}