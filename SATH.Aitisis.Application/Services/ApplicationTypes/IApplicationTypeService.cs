namespace SATH.Aitisis.App.Services.ApplicationTypes
{
    //use-case–application abstraction for managing application types in the system
    public interface IApplicationTypeService
    {
        Task<IReadOnlyCollection<ApplicationTypeListItem>>
            GetApplicationTypesAsync();

        Task<CreateApplicationTypeResult>
            CreateApplicationTypeAsync(CreateApplicationTypeRequest request);

        Task<ApplicationTypeDetails?> GetDetailsAsync(int id);

        Task<bool> AddFieldAsync(AddApplicationTypeFieldRequest request);

        Task<bool> AddDocumentRequirementAsync(AddDocumentRequirementRequest request);

        Task<bool> DeactivateAsync(int id);

        Task<bool> ReactivateAsync(int id);
    }
}