using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SATH.Aitisis.Domain.Entities;

namespace SATH.Aitisis.Infrastructure.Persistence.Configurations
{
    public class DocumentRequirementConfiguration
        : IEntityTypeConfiguration<DocumentRequirement>
    {
        public void Configure(
            EntityTypeBuilder<DocumentRequirement> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.IsRequired)
                .IsRequired();

            builder.Property(x => x.DisplayOrder)
                .IsRequired();

            builder.HasOne(x => x.ApplicationType)
                .WithMany(x => x.DocumentRequirements)
                .HasForeignKey(x => x.ApplicationTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Documents)
                .WithOne(x => x.DocumentRequirement)
                .HasForeignKey(x => x.DocumentRequirementId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}