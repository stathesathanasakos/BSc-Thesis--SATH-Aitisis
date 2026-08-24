using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SATH.Aitisis.Domain.Entities;

namespace SATH.Aitisis.Infrastructure.Persistence.Configurations
{
    public class ApplicationTypeConfiguration
        : IEntityTypeConfiguration<ApplicationType>
    {
        // configure the ApplicationType entity and its relationships with other entities
        public void Configure(EntityTypeBuilder<ApplicationType> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.FeeAmount)
                .HasPrecision(10, 2);

            builder.HasMany(x => x.Applications)
                .WithOne(x => x.ApplicationType)
                .HasForeignKey(x => x.ApplicationTypeId)
                .OnDelete(DeleteBehavior.Restrict);    // Prevent cascading delete to avoid deleting applications when an application type is deleted
        }
    }
}
