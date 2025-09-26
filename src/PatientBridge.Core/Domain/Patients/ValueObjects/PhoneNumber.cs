// src/PatientBridge.Core/Domain/Patients/ValueObjects/PhoneNumber.cs
using System.Text.RegularExpressions;
using PatientBridge.Core.Common;

namespace PatientBridge.Core.Domain.Patients.ValueObjects;

public record PhoneNumber
{
    private static readonly Regex PhoneRegex = new(@"^\+[1-9]\d{1,14}$", RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result.Failure<PhoneNumber>("Phone number is required");

        var cleanedPhone = phoneNumber.Trim();

        if (!PhoneRegex.IsMatch(cleanedPhone))
            return Result.Failure<PhoneNumber>("Phone number must be in international format (e.g., +1234567890)");

        return Result.Success(new PhoneNumber(cleanedPhone));
    }

    public override string ToString() => Value;
}
