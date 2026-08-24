using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SATH.Aitisis.Domain.Entities;

namespace SATH.Aitisis.Infrastructure.Persistence.Configurations
{
    public class UserNotificationConfiguration
        : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Message)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.Type)
                .IsRequired();

            builder.Property(x => x.IsRead)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.Application)
                .WithMany(x => x.UserNotifications)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.SetNull);       // Set the ApplicationId to null when the related Application is deleted
        }
    }
}
