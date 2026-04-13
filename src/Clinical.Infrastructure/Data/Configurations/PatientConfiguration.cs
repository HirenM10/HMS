using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Infrastructure.Data.Configurations;

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Gender).HasConversion<int>().IsRequired();
        builder.Property(x => x.DateOfBirth)
            .HasConversion(x => x.ToDateTime(TimeOnly.MinValue), x => DateOnly.FromDateTime(x))
            .HasColumnType("date")
            .IsRequired();

        builder.OwnsOne(
            x => x.ContactNumber,
            contact =>
            {
                contact.Property(x => x.Value)
                    .HasColumnName("ContactNumber")
                    .HasMaxLength(20)
                    .IsRequired();
            });

        builder.Navigation(x => x.ContactNumber).IsRequired();

        builder.Metadata.FindNavigation(nameof(Patient.Appointments))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Appointments)
            .WithOne(x => x.Patient)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
