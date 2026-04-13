using Clinical.Domain.Common;
using Clinical.Domain.Enums;

namespace Clinical.Domain.Entities;

public sealed class Appointment : BaseEntity
{
    private Appointment()
    {
    }

    private Appointment(Guid id, Guid patientId, string doctorName, DateTime appointmentDate, AppointmentStatus status)
    {
        Id = id;
        PatientId = patientId;
        DoctorName = doctorName;
        AppointmentDate = appointmentDate;
        Status = status;
    }

    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public string DoctorName { get; private set; } = string.Empty;
    public DateTime AppointmentDate { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public Patient? Patient { get; private set; }

    public static Appointment Schedule(Guid patientId, string doctorName, DateTime appointmentDate)
    {
        if (string.IsNullOrWhiteSpace(doctorName))
        {
            throw new ArgumentException("Doctor name is required.", nameof(doctorName));
        }

        return new Appointment(Guid.NewGuid(), patientId, doctorName.Trim(), appointmentDate, AppointmentStatus.Scheduled);
    }

    public void UpdateDetails(string doctorName, DateTime appointmentDate, AppointmentStatus status)
    {
        if (string.IsNullOrWhiteSpace(doctorName))
        {
            throw new ArgumentException("Doctor name is required.", nameof(doctorName));
        }

        DoctorName = doctorName.Trim();
        AppointmentDate = appointmentDate;
        Status = status;
    }
}
