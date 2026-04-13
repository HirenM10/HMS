namespace Clinical.Application.Abstractions.Messaging;

public interface ICacheInvalidator
{
    void InvalidatePatients();
    void InvalidatePatient(Guid patientId);
    void InvalidateAppointments();
    void InvalidateAppointment(Guid appointmentId);
}
