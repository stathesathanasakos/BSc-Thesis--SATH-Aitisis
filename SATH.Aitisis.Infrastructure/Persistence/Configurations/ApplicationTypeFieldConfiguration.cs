using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SATH.Aitisis.Domain.Entities;

namespace SATH.Aitisis.Infrastructure.Persistence.Configurations
{
    public class ApplicationTypeFieldConfiguration
        : IEntityTypeConfiguration<ApplicationTypeField>
    {
        public void Configure(
            EntityTypeBuilder<ApplicationTypeField> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Label)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.FieldType)
                .IsRequired();

            builder.Property(x => x.IsRequired)
                .IsRequired();

            builder.Property(x => x.DisplayOrder)
                .IsRequired();

            // Field technical names must be unique
            // within the same application type.
            builder.HasIndex(x => new
            {
                x.ApplicationTypeId,
                x.Name
            })
            .IsUnique();

            builder.HasOne(x => x.ApplicationType)
                .WithMany(x => x.Fields)
                .HasForeignKey(x => x.ApplicationTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Values)
                .WithOne(x => x.ApplicationTypeField)
                .HasForeignKey(x => x.ApplicationTypeFieldId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}