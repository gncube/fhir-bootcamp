// src/PatientBridge.Core/Application/Patients/DTOs/PatientDto.cs
using PatientBridge.Core.Domain.Patients;

namespace PatientBridge.Core.Application.Patients.DTOs;

public record PatientDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    Gender Gender,
    DateOnly DateOfBirth,
    int Age,
    string PhoneNumber,
    DateTime CreatedAt,
    DateTime ModifiedAt,
    string? FhirId = null)
{
    public static PatientDto FromDomain(Patient patient) => new(
        patient.Id,
        patient.Name.FirstName,
        patient.Name.LastName,
        patient.Name.FullName,
        patient.Gender,
        patient.DateOfBirth,
        patient.GetAge(),
        patient.PhoneNumber.Value,
        patient.CreatedAt,
        patient.ModifiedAt,
        patient.FhirId
    );
}
