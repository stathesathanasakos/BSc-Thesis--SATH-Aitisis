using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SATH.Aitisis.Domain.Entities;
using DomainApplication = SATH.Aitisis.Domain.Entities.Application;


namespace SATH.Aitisis.Infrastructure.Persistence.Configurations
{
    public class ApplicationConfiguration
        : IEntityTypeConfiguration<DomainApplication>
    {
        public void Configure(EntityTypeBuilder<DomainApplication> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CitizenId)
                .IsRequired();

            builder.Property(x => x.ProtocolNumber)
                .HasMaxLength(50);

            // unique index on ProtocolNumber to ensure that no two applications can have the same protocol number
            builder.HasIndex(x => x.ProtocolNumber)
                .IsUnique();      

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.SubmissionDate)
                .IsRequired();

            builder.HasOne(x => x.ApplicationType)
                .WithMany(x => x.Applications)
                .HasForeignKey(x => x.ApplicationTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Documents)
                .WithOne(x => x.Application)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            //setNull because if an application is deleted, the UserNotifications should not be deleted but their ApplicationId should be set to null
            builder.HasMany(x => x.UserNotifications)
                .WithOne(x => x.Application)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Payment)
                .WithOne(x => x.Application)
                .HasForeignKey<Payment>(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}