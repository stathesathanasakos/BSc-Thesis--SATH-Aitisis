using SATH.Aitisis.Domain.Entities;

// Repository pattern interface for managing ApplicationType entities in the database
namespace SATH.Aitisis.App.Services.ApplicationTypes
{
    public interface IApplicationTypeRepository
    {
        Task<IReadOnlyCollection<ApplicationType>> GetAllAsync();

        Task<ApplicationType?> GetByIdWithConfigurationAsync(int id);

        Task<bool> ExistsByNameAsync(string name);

        Task AddAsync(ApplicationType applicationType);

        Task AddFieldAsync(ApplicationTypeField field);

        Task AddDocumentRequirementAsync(DocumentRequirement documentRequirement);

        Task SaveChangesAsync();
    }
}