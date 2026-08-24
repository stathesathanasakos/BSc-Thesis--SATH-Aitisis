using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SATH.Aitisis.Infrastructure.Identity;
using DomainApplication = SATH.Aitisis.Domain.Entities.Application;
using Document = SATH.Aitisis.Domain.Entities.Document;
using UserNotification = SATH.Aitisis.Domain.Entities.UserNotification;


namespace SATH.Aitisis.Infrastructure.Persistence.Configurations
{
    public class ApplicationUserConfiguration
        : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();


            // Relationships

            //.WithOne() because ApplicationUser is the principal entity in these relationships, and the related entities (DomainApplication, Document, UserNotification)
            // are dependent entities that reference ApplicationUser through foreign keys
            // THIS WAY I keep Domain independent of Identity 

            // Citizen who submitted the application
            builder.HasMany<DomainApplication>()
                .WithOne()
                .HasForeignKey(x => x.CitizenId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee who reviewed the application
            builder.HasMany<DomainApplication>()
                .WithOne()
                .HasForeignKey(x => x.ReviewedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User who uploaded the document
            builder.HasMany<Document>()
                .WithOne()
                .HasForeignKey(x => x.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User who owns the in-app notification
            builder.HasMany<UserNotification>()
                .WithOne()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}