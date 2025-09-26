// src/PatientBridge.Core/Domain/Patients/Patient.cs
using PatientBridge.Core.Common;
using PatientBridge.Core.Domain.Patients.ValueObjects;

namespace PatientBridge.Core.Domain.Patients;

public class Patient
{
    public Guid Id { get; private set; }
    public PatientName Name { get; private set; }
    public Gender Gender { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ModifiedAt { get; private set; }

    // For FHIR integration
    public string? FhirId { get; private set; }

    private Patient() { } // EF Core constructor

    private Patient(PatientName name, Gender gender, DateOnly dateOfBirth, PhoneNumber phoneNumber)
    {
        Id = Guid.NewGuid();
        Name = name;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
        CreatedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
    }

    public static Result<Patient> Create(PatientName name, Gender gender, DateOnly dateOfBirth, PhoneNumber phoneNumber)
    {
        if (dateOfBirth > DateOnly.FromDateTime(DateTime.Now))
            return Result.Failure<Patient>("Date of birth cannot be in the future");

        var patient = new Patient(name, gender, dateOfBirth, phoneNumber);
        return Result.Success(patient);
    }

    public Result UpdateDetails(PatientName name, Gender gender, DateOnly dateOfBirth, PhoneNumber phoneNumber)
    {
        if (dateOfBirth > DateOnly.FromDateTime(DateTime.Now))
            return Result.Failure("Date of birth cannot be in the future");

        Name = name;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
        ModifiedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public void SetFhirId(string fhirId)
    {
        FhirId = fhirId;
        ModifiedAt = DateTime.UtcNow;
    }

    public int GetAge()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - DateOfBirth.Year;

        if (DateOfBirth > today.AddYears(-age))
            age--;

        return age;
    }
}
