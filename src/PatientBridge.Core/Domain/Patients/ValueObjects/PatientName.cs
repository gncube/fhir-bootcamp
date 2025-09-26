
// src/PatientBridge.Core/Domain/Patients/ValueObjects/PatientName.cs
using PatientBridge.Core.Common;

namespace PatientBridge.Core.Domain.Patients.ValueObjects;

public record PatientName
{
    public string FirstName { get; }
    public string LastName { get; }
    public string FullName => $"{FirstName} {LastName}";

    private PatientName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<PatientName> Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<PatientName>("First name is required and cannot be empty");

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<PatientName>("Last name is required and cannot be empty");

        return Result.Success(new PatientName(firstName.Trim(), lastName.Trim()));
    }
}
