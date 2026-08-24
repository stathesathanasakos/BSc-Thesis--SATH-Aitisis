namespace SATH.Aitisis.App.Services.Employees
{
    public interface IEmployeeManagementService
    {
        Task<CreateEmployeeResult> CreateEmployeeAsync(CreateEmployeeRequest request);


        Task<IReadOnlyCollection<EmployeeListItem>> GetEmployeesAsync();


        Task<bool> DeactivateEmployeeAsync(string employeeId);


        Task<bool> ReactivateEmployeeAsync(string employeeId);
    }
}