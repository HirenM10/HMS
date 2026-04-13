using Clinical.Domain.Common;
using Clinical.Domain.Enums;
using Clinical.Domain.Events;
using Clinical.Domain.ValueObjects;

namespace Clinical.Domain.Entities;

public sealed class Patient : AggregateRoot
{
    private readonly List<Appointment> _appointments = [];

    private Patient()
    {
    }

    private Patient(Guid id, string firstName, string lastName, DateOnly dateOfBirth, Gender gender, ContactNumber contactNumber)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        ContactNumber = contactNumber;
    }

    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateOnly DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public ContactNumber ContactNumber { get; private set; } = ContactNumber.Create("+10000000000");
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    public static Patient Create(string firstName, string lastName, DateOnly dateOfBirth, Gender gender, string contactNumber)
    {
        ValidateName(firstName, nameof(firstName));
        ValidateName(lastName, nameof(lastName));

        return new Patient(
            Guid.NewGuid(),
            firstName.Trim(),
            lastName.Trim(),
            dateOfBirth,
            gender,
            ContactNumber.Create(contactNumber));
    }

    public void UpdateDetails(string firstName, string lastName, DateOnly dateOfBirth, Gender gender, string contactNumber)
    {
        ValidateName(firstName, nameof(firstName));
        ValidateName(lastName, nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DateOfBirth = dateOfBirth;
        Gender = gender;
        ContactNumber = ContactNumber.Create(contactNumber);
    }

    public Appointment ScheduleAppointment(string doctorName, DateTime appointmentDate)
    {
        var appointment = Appointment.Schedule(Id, doctorName, appointmentDate);
        _appointments.Add(appointment);
        AddDomainEvent(new AppointmentCreatedDomainEvent(Id, appointment.Id, appointmentDate, doctorName));
        return appointment;
    }

    public void UpdateAppointment(Guid appointmentId, string doctorName, DateTime appointmentDate, AppointmentStatus status)
    {
        var appointment = _appointments.SingleOrDefault(x => x.Id == appointmentId)
            ?? throw new InvalidOperationException($"Appointment '{appointmentId}' was not found for patient '{Id}'.");

        appointment.UpdateDetails(doctorName, appointmentDate, status);
    }

    public void RemoveAppointment(Guid appointmentId)
    {
        var appointment = _appointments.SingleOrDefault(x => x.Id == appointmentId)
            ?? throw new InvalidOperationException($"Appointment '{appointmentId}' was not found for patient '{Id}'.");

        _appointments.Remove(appointment);
    }

    private static void ValidateName(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Name is required.", paramName);
        }
    }
}
