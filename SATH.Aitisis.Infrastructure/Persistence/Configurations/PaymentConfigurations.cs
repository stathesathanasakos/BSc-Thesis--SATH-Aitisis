using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SATH.Aitisis.Domain.Entities;

namespace SATH.Aitisis.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration
        : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.Provider)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ExternalPaymentId)
                .HasMaxLength(150);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // unique index on ApplicationId to ensure one-to-one relationship between Payment and Application
            builder.HasIndex(x => x.ApplicationId)
                .IsUnique();

            builder.HasOne(x => x.Application)
                .WithOne(x => x.Payment)
                .HasForeignKey<Payment>(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
