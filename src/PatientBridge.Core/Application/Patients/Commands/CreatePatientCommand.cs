// src/PatientBridge.Core/Application/Patients/Commands/CreatePatientCommand.cs
using PatientBridge.Core.Domain.Patients;

namespace PatientBridge.Core.Application.Patients.Commands;

public record CreatePatientCommand(
    string FirstName,
    string LastName,
    Gender Gender,
    DateOnly DateOfBirth,
    string PhoneNumber);
