// src/PatientBridge.Core/Application/Patients/Commands/UpdatePatientCommand.cs
using PatientBridge.Core.Domain.Patients;

namespace PatientBridge.Core.Application.Patients.Commands;

public record UpdatePatientCommand(
    Guid Id,
    string FirstName,
    string LastName,
    Gender Gender,
    DateOnly DateOfBirth,
    string PhoneNumber);
