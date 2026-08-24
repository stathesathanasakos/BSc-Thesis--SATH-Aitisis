using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SATH.Aitisis.Domain.Entities;

namespace SATH.Aitisis.Infrastructure.Persistence.Configurations
{
    public class DocumentConfiguration
        : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UploadedByUserId)
                .IsRequired();

            builder.Property(x => x.OriginalFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.StoredFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.FileSize)
                .IsRequired();

            builder.Property(x => x.UploadDate)
                .IsRequired();

            builder.HasOne(x => x.Application)
                .WithMany(x => x.Documents)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);     // if the application is deleted, also delete the associated documents metadata

            // Configure the relationship with DocumentRequirement
            builder.HasOne(x => x.DocumentRequirement)
                .WithMany(x => x.Documents)
                .HasForeignKey(x => x.DocumentRequirementId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}