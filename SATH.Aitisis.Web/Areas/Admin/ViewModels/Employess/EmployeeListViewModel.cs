//namespace SATH.Aitisis.Web.Areas.Admin.ViewModels.Employees {}
using System;

namespace SATH.Aitisis.Web.Areas.Admin.ViewModels.Employees
{
    public sealed class EmployeeListViewModel
    {
        public string Id { get; init; } = string.Empty;

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public bool IsActive { get; init; }

        public DateTime CreatedAt { get; init; }
    }
}