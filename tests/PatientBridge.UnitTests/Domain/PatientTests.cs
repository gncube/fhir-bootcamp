// tests/PatientBridge.UnitTests/Domain/PatientTests.cs
using FluentAssertions;
using PatientBridge.Core.Domain.Patients;
using PatientBridge.Core.Domain.Patients.ValueObjects;

namespace PatientBridge.UnitTests.Domain;

public class PatientTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreatePatient()
    {
        // Arrange
        var patientName = PatientName.Create("John", "Doe");
        var gender = Gender.Male;
        var dateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30));
        var phoneNumber = PhoneNumber.Create("+1234567890");

        // Act
        var result = Patient.Create(patientName, gender, dateOfBirth, phoneNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var patient = result.Value;
        patient.Name.Should().Be(patientName);
        patient.Gender.Should().Be(gender);
        patient.DateOfBirth.Should().Be(dateOfBirth);
        patient.PhoneNumber.Should().Be(phoneNumber);
        patient.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithFutureDateOfBirth_ShouldFail()
    {
        // Arrange
        var patientName = PatientName.Create("John", "Doe");
        var gender = Gender.Male;
        var futureDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
        var phoneNumber = PhoneNumber.Create("+1234567890");

        // Act
        var result = Patient.Create(patientName, gender, futureDate, phoneNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Date of birth cannot be in the future");
    }

    [Fact]
    public void UpdateDetails_WithValidData_ShouldUpdatePatient()
    {
        // Arrange
        var patient = CreateValidPatient();
        var newName = PatientName.Create("Jane", "Smith");
        var newPhoneNumber = PhoneNumber.Create("+9876543210");

        // Act
        var result = patient.UpdateDetails(newName, Gender.Female, patient.DateOfBirth, newPhoneNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        patient.Name.Should().Be(newName);
        patient.Gender.Should().Be(Gender.Female);
        patient.PhoneNumber.Should().Be(newPhoneNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_WithInvalidFirstName_ShouldFail(string firstName)
    {
        // Act & Assert
        var result = PatientName.Create(firstName, "Doe");
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("First name is required");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_WithInvalidLastName_ShouldFail(string lastName)
    {
        // Act & Assert
        var result = PatientName.Create("John", lastName);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Last name is required");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("invalid-phone")]
    [InlineData("")]
    public void Create_WithInvalidPhoneNumber_ShouldFail(string phoneNumber)
    {
        // Act & Assert
        var result = PhoneNumber.Create(phoneNumber);
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData("+1234567890")]
    [InlineData("+44123456789")]
    [InlineData("+919876543210")]
    public void Create_WithValidPhoneNumber_ShouldSucceed(string phoneNumber)
    {
        // Act & Assert
        var result = PhoneNumber.Create(phoneNumber);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Patient_ShouldHaveUniqueId()
    {
        // Arrange & Act
        var patient1 = CreateValidPatient();
        var patient2 = CreateValidPatient();

        // Assert
        patient1.Id.Should().NotBe(patient2.Id);
    }

    [Fact]
    public void Patient_ShouldTrackCreationTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var patient = CreateValidPatient();
        var afterCreation = DateTime.UtcNow;

        // Assert
        patient.CreatedAt.Should().BeAfter(beforeCreation);
        patient.CreatedAt.Should().BeBefore(afterCreation);
    }

    [Fact]
    public void Patient_ShouldTrackLastModificationTime()
    {
        // Arrange
        var patient = CreateValidPatient();
        var originalModifiedAt = patient.ModifiedAt;

        Thread.Sleep(10); // Ensure time difference

        // Act
        var newName = PatientName.Create("Updated", "Name");
        patient.UpdateDetails(newName, patient.Gender, patient.DateOfBirth, patient.PhoneNumber);

        // Assert
        patient.ModifiedAt.Should().BeAfter(originalModifiedAt);
    }

    private static Patient CreateValidPatient()
    {
        var name = PatientName.Create("John", "Doe");
        var gender = Gender.Male;
        var dateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30));
        var phoneNumber = PhoneNumber.Create("+1234567890");

        return Patient.Create(name, gender, dateOfBirth, phoneNumber).Value;
    }
}
