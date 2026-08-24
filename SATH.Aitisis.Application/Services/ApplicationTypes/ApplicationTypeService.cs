using SATH.Aitisis.Domain.Entities;


namespace SATH.Aitisis.App.Services.ApplicationTypes
{
    // Handles application-type use cases and business rules 
    // for managing application types in the system but not directly interacting with the database
    public sealed class ApplicationTypeService: IApplicationTypeService
    {
        private readonly IApplicationTypeRepository _repository;

        public ApplicationTypeService(IApplicationTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyCollection<ApplicationTypeListItem>> GetApplicationTypesAsync()
        {
            var applicationTypes =
                await _repository.GetAllAsync();

            return applicationTypes
                .OrderBy(x => x.Name)
                .Select(x => new ApplicationTypeListItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    RequiresPayment = x.RequiresPayment,
                    FeeAmount = x.FeeAmount,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToArray();
        }

        public async Task<CreateApplicationTypeResult>CreateApplicationTypeAsync(CreateApplicationTypeRequest request)
        {
            var name = request.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                return Failure("Application type name is required.");
            }

            if (await _repository.ExistsByNameAsync(name))
            {
                return Failure("An application type with this name already exists.");
            }

            decimal? feeAmount = null;

            if (request.RequiresPayment)
            {
                if (!request.FeeAmount.HasValue ||
                    request.FeeAmount.Value <= 0)
                {
                    return Failure("A positive fee amount is required when payment is enabled.");
                }

                feeAmount = request.FeeAmount.Value;
            }

            var applicationType = new ApplicationType
            {
                Name = name,
                Description =
                    string.IsNullOrWhiteSpace(request.Description)
                        ? null
                        : request.Description.Trim(),

                RequiresPayment = request.RequiresPayment,
                FeeAmount = feeAmount,

                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(applicationType);
            await _repository.SaveChangesAsync();

            return new CreateApplicationTypeResult
            {
                Succeeded = true,
                ApplicationTypeId = applicationType.Id
            };
        }


        private static CreateApplicationTypeResult Failure(string error)
        {
            return new CreateApplicationTypeResult
            {
                Succeeded = false,
                Errors = new[] { error }
            };
        }



        // Retrieves detailed information about a specific application type – including its fields and document requirements
        public async Task<ApplicationTypeDetails?> GetDetailsAsync(int id)
        {
            var applicationType =
                await _repository.GetByIdWithConfigurationAsync(id);

            if (applicationType == null)
            {
                return null;
            }

            return new ApplicationTypeDetails
            {
                Id = applicationType.Id,
                Name = applicationType.Name,
                Description = applicationType.Description,
                RequiresPayment = applicationType.RequiresPayment,
                FeeAmount = applicationType.FeeAmount,
                IsActive = applicationType.IsActive,

                Fields = applicationType.Fields
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new ApplicationTypeFieldItem
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Label = x.Label,
                        FieldType = x.FieldType,
                        IsRequired = x.IsRequired,
                        DisplayOrder = x.DisplayOrder
                    })
                    .ToArray(),

                DocumentRequirements =
                    applicationType.DocumentRequirements
                        .OrderBy(x => x.DisplayOrder)
                        .Select(x => new DocumentRequirementItem
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            IsRequired = x.IsRequired,
                            DisplayOrder = x.DisplayOrder
                        })
                        .ToArray()
            };
        }




        public async Task<bool> AddFieldAsync(AddApplicationTypeFieldRequest request)
        {
            var applicationType =
                await _repository.GetByIdWithConfigurationAsync(
                    request.ApplicationTypeId);

            if (applicationType == null)
            {
                return false;
            }

            var name = request.Name.Trim();
            var label = request.Label.Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(label))
            {
                return false;
            }

            var duplicateName = applicationType.Fields
                .Any(x => x.Name.Equals(
                    name,
                    StringComparison.OrdinalIgnoreCase));

            if (duplicateName)
            {
                return false;
            }

            var nextDisplayOrder =
                applicationType.Fields.Count == 0
                    ? 1
                    : applicationType.Fields.Max(x => x.DisplayOrder) + 1;

            var field = new ApplicationTypeField
            {
                ApplicationTypeId = applicationType.Id,
                Name = name,
                Label = label,
                FieldType = request.FieldType,
                IsRequired = request.IsRequired,
                DisplayOrder = nextDisplayOrder
            };

            await _repository.AddFieldAsync(field);
            await _repository.SaveChangesAsync();

            return true;
        }



        public async Task<bool> AddDocumentRequirementAsync(AddDocumentRequirementRequest request)
        {
            var applicationType =
                await _repository.GetByIdWithConfigurationAsync(
                    request.ApplicationTypeId);

            if (applicationType == null)
            {
                return false;
            }

            var name = request.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            var duplicateName = applicationType.DocumentRequirements
                .Any(x => x.Name.Equals(
                    name,
                    StringComparison.OrdinalIgnoreCase));

            if (duplicateName)
            {
                return false;
            }

            var nextDisplayOrder = applicationType.DocumentRequirements.Count == 0
                    ? 1
                    : applicationType.DocumentRequirements
                        .Max(x => x.DisplayOrder) + 1;

            var documentRequirement = new DocumentRequirement
            {
                ApplicationTypeId = applicationType.Id,
                Name = name,
                Description =
                    string.IsNullOrWhiteSpace(request.Description)
                        ? null
                        : request.Description.Trim(),
                IsRequired = request.IsRequired,
                DisplayOrder = nextDisplayOrder
            };

            await _repository.AddDocumentRequirementAsync(
                documentRequirement);

            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
