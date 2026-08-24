using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SATH.Aitisis.Domain.Entities;
using SATH.Aitisis.Infrastructure.Identity;
using DomainApplication = SATH.Aitisis.Domain.Entities.Application;

namespace SATH.Aitisis.Infrastructure.Persistence
{
    // Main EF Core database context for both Identity
    // and SATH-Aitisis application data
    public class SathAitisisDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public SathAitisisDbContext(
            DbContextOptions<SathAitisisDbContext> options)
            : base(options)
        {
        }

        public DbSet<DomainApplication> Applications { get; set; }

        public DbSet<ApplicationType> ApplicationTypes { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<UserNotification> UserNotifications { get; set; }

        public DbSet<ApplicationTypeField> ApplicationTypeFields { get; set; }

        public DbSet<ApplicationFieldValue> ApplicationFieldValues { get; set; }

        public DbSet<DocumentRequirement> DocumentRequirements { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Applies the ASP.NET Core Identity model configuration
            base.OnModelCreating(builder);

            // Automatically loads all EF Core entity configurations defined in the Infrastructure assembly
            builder.ApplyConfigurationsFromAssembly(
                typeof(SathAitisisDbContext).Assembly);
        }
    }
}