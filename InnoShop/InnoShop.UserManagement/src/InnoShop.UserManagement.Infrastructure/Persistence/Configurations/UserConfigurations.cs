using InnoShop.UserManagement.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnoShop.UserManagement.Infrastructure.Persistence.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.Property(u => u.Email)
            .HasMaxLength(400)
            .IsRequired()
            .HasColumnName("email")
            .HasConversion(
                email => email.Value,
                value => new Email(value));
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.IsActive);
        builder.Property(u => u.IsEmailVerified);
        builder.Property(u => u.EmailVerificationToken);
        builder.Property(u => u.EmailVerificationTokenExpiration);
        builder.Property(u => u.PasswordResetToken)
            .IsRequired(false);
        builder.Property(u => u.PasswordResetTokenExpiration)
            .IsRequired(false);

        builder.OwnsOne(u => u.RatingSummary, ratingSummary =>
        {
            ratingSummary.Property(x => x.TotalScore)
                .HasColumnName("total_score");
            ratingSummary.Property(x => x.NumberOfReviews)
                .HasColumnName("number_of_reviews");
        });

        builder.OwnsOne(u => u.UserProfile, profileBuilder =>
        {
            profileBuilder.Property(p => p.FirstName)
                .HasMaxLength(200)
                .HasConversion(n => n.Value, v => new FirstName(v))
                .HasColumnName("first_name");

            profileBuilder.Property(p => p.LastName)
                .HasMaxLength(200)
                .HasConversion(n => n.Value, v => new LastName(v))
                .HasColumnName("last_name");

            profileBuilder.Property(p => p.AvatarUrl)
                .HasMaxLength(2048)
                .HasConversion(a => a.Value, v => new AvatarUrl(v))
                .HasColumnName("avatar_url");

            profileBuilder.Property(p => p.PhoneNumber)
                .HasMaxLength(50)
                .HasConversion(p => p.Value, v => new PhoneNumber(v))
                .HasColumnName("phone_number");

            profileBuilder.OwnsOne(p => p.Location, locationBuilder =>
            {
                locationBuilder.Property(l => l.Country)
                    .HasConversion(
                        c => c.Name,
                        n => Country.FromName(n, false))
                    .HasMaxLength(100)
                    .HasColumnName("country");

                locationBuilder.Property(l => l.State).HasMaxLength(100).HasColumnName("state");
                locationBuilder.Property(l => l.City).HasMaxLength(100).HasColumnName("city");
            });
        });

        builder.Navigation(u => u.UserProfile).IsRequired(false);

        builder.Property("_passwordHash")
            .HasColumnName("password_hash");

        builder.HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity("user_roles");
    }
}