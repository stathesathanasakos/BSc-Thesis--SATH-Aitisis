using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SATH.Aitisis.Domain.Entities;

namespace SATH.Aitisis.Infrastructure.Persistence.Configurations
{
    public class ApplicationFieldValueConfiguration
        : IEntityTypeConfiguration<ApplicationFieldValue>
    {
        public void Configure(
            EntityTypeBuilder<ApplicationFieldValue> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(2000);

            builder.HasOne(x => x.Application)
                .WithMany(x => x.FieldValues)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ApplicationTypeField)
                .WithMany(x => x.Values)
                .HasForeignKey(x => x.ApplicationTypeFieldId)
                .OnDelete(DeleteBehavior.Restrict);

            // One application can have only one value
            // for each defined field.
            builder.HasIndex(x => new
            {
                x.ApplicationId,
                x.ApplicationTypeFieldId
            })
            .IsUnique();
        }
    }
}