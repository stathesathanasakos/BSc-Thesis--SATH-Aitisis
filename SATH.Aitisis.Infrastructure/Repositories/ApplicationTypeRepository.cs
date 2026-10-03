using Microsoft.EntityFrameworkCore;
using SATH.Aitisis.App.Services.ApplicationTypes;
using SATH.Aitisis.Domain.Entities;
using SATH.Aitisis.Infrastructure.Persistence;

namespace SATH.Aitisis.Infrastructure.Repositories.ApplicationTypes
{
    // EF Core implementation of application type persistence 
    // This is the concrete implementation of the IApplicationTypeRepository interface
    // that uses Entity Framework Core to interact with the database for managing ApplicationType entities

    //Dependency Injection: This class is designed to be injected into services or controllers that need to interact with application types in the database
    //It takes a SathAitisisDbContext instance as a dependency, which is provided by the DI container

    public sealed class ApplicationTypeRepository: IApplicationTypeRepository
    {
        private readonly SathAitisisDbContext _dbContext;

        public ApplicationTypeRepository(SathAitisisDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<IReadOnlyCollection<ApplicationType>> GetAllAsync()
        {
            // AsNoTracking because I am only reading data and not modifying it
            // that way I Improve performance
            return await _dbContext.ApplicationTypes
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToArrayAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbContext.ApplicationTypes.AnyAsync(x => x.Name == name);
        }

        public async Task AddAsync(ApplicationType applicationType)
        {
            await _dbContext.ApplicationTypes.AddAsync(applicationType);
        }



        // Retrieves an application type by its ID, including its associated fields and document requirements
        public async Task<ApplicationType?>GetByIdWithConfigurationAsync(int id)
        {
            return await _dbContext.ApplicationTypes
                .Include(x => x.Fields)
                .Include(x => x.DocumentRequirements)
                .FirstOrDefaultAsync(x => x.Id == id);
        }


        // Adds a new field to an application type
        public async Task AddFieldAsync(ApplicationTypeField field)
        {
            await _dbContext.ApplicationTypeFields.AddAsync(field);
        }

        public async Task AddDocumentRequirementAsync(DocumentRequirement documentRequirement)
        {
            await _dbContext.DocumentRequirements.AddAsync(documentRequirement);
        }


        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }


        public async Task<ApplicationType?> GetByIdForUpdateAsync(int id)
        {
            return await _dbContext.ApplicationTypes.FirstOrDefaultAsync(x => x.Id == id);
        }

    }
}