// tests/PatientBridge.UnitTests/Application/PatientServiceTests.cs
using FluentAssertions;
using Moq;
using Xunit;
using PatientBridge.Core.Application.Patients;
using PatientBridge.Core.Application.Patients.Commands;
using PatientBridge.Core.Application.Patients.Queries;
using PatientBridge.Core.Common;
using PatientBridge.Core.Domain.Patients;
using PatientBridge.Core.Domain.Patients.ValueObjects;

namespace PatientBridge.UnitTests.Application;

public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _mockRepository;
    private readonly PatientService _patientService;

    public PatientServiceTests()
    {
        _mockRepository = new Mock<IPatientRepository>();
        _patientService = new PatientService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreatePatient_WithValidData_ShouldSucceed()
    {
        // Arrange
        var command = new CreatePatientCommand(
            "John",
            "Doe",
            Gender.Male,
            DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
            "+1234567890"
        );

        var createdPatient = CreateValidPatient();
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Patient>()))
            .ReturnsAsync(Result.Success(createdPatient));

        // Act
        var result = await _patientService.CreatePatientAsync(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dto = result.Value;
        dto.FirstName.Should().Be(command.FirstName);
        dto.LastName.Should().Be(command.LastName);
        dto.Gender.Should().Be(command.Gender);
        dto.DateOfBirth.Should().Be(command.DateOfBirth);
        dto.PhoneNumber.Should().Be(command.PhoneNumber);

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Once);
    }

    [Fact]
    public async Task CreatePatient_WithInvalidName_ShouldFail()
    {
        // Arrange
        var command = new CreatePatientCommand(
            "", // Invalid first name
            "Doe",
            Gender.Male,
            DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
            "+1234567890"
        );

        // Act
        var result = await _patientService.CreatePatientAsync(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("First name is required");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Never);
    }

    [Fact]
    public async Task GetAllPatients_ShouldReturnAllPatients()
    {
        // Arrange
        var patients = new List<Patient> { CreateValidPatient(), CreateValidPatient() };
        _mockRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(Result.Success<IEnumerable<Patient>>(patients));

        // Act
        var result = await _patientService.GetAllPatientsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task SearchPatients_ByName_ShouldReturnMatchingPatients()
    {
        // Arrange
        var query = new SearchPatientsQuery("John");
        var matchingPatients = new List<Patient> { CreateValidPatient() };

        _mockRepository
            .Setup(r => r.SearchByNameAsync("John"))
            .ReturnsAsync(Result.Success<IEnumerable<Patient>>(matchingPatients));

        // Act
        var result = await _patientService.SearchPatientsAsync(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        _mockRepository.Verify(r => r.SearchByNameAsync("John"), Times.Once);
    }

    [Fact]
    public async Task SearchPatients_ByNameOrPhone_ShouldReturnMatchingPatients()
    {
        // Arrange
        var query = new SearchPatientsQuery("123", IncludePhoneSearch: true);
        var matchingPatients = new List<Patient> { CreateValidPatient() };

        _mockRepository
            .Setup(r => r.SearchByNameOrPhoneAsync("123"))
            .ReturnsAsync(Result.Success<IEnumerable<Patient>>(matchingPatients));

        // Act
        var result = await _patientService.SearchPatientsAsync(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        _mockRepository.Verify(r => r.SearchByNameOrPhoneAsync("123"), Times.Once);
    }

    [Fact]
    public async Task UpdatePatient_WithValidData_ShouldSucceed()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var existingPatient = CreateValidPatient();
        var command = new UpdatePatientCommand(
            patientId,
            "Jane",
            "Smith",
            Gender.Female,
            DateOnly.FromDateTime(DateTime.Now.AddYears(-25)),
            "+9876543210"
        );

        _mockRepository
            .Setup(r => r.GetByIdAsync(patientId))
            .ReturnsAsync(Result.Success<Patient?>(existingPatient));

        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Patient>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _patientService.UpdatePatientAsync(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(r => r.GetByIdAsync(patientId), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Patient>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePatient_NonExistentPatient_ShouldFail()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var command = new UpdatePatientCommand(
            patientId,
            "Jane",
            "Smith",
            Gender.Female,
            DateOnly.FromDateTime(DateTime.Now.AddYears(-25)),
            "+9876543210"
        );

        _mockRepository
            .Setup(r => r.GetByIdAsync(patientId))
            .ReturnsAsync(Result.Success<Patient?>(null));

        // Act
        var result = await _patientService.UpdatePatientAsync(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Patient not found");
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Patient>()), Times.Never);
    }

    [Fact]
    public async Task DeletePatient_ExistingPatient_ShouldSucceed()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var existingPatient = CreateValidPatient();

        _mockRepository
            .Setup(r => r.GetByIdAsync(patientId))
            .ReturnsAsync(Result.Success<Patient?>(existingPatient));

        _mockRepository
            .Setup(r => r.DeleteAsync(patientId))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _patientService.DeletePatientAsync(patientId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(r => r.DeleteAsync(patientId), Times.Once);
    }

    private static Patient CreateValidPatient()
    {
        var nameResult = PatientName.Create("John", "Doe");
        nameResult.IsSuccess.Should().BeTrue();
        var name = nameResult.Value;
        var gender = Gender.Male;
        var dateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30));
        var phoneNumberResult = PhoneNumber.Create("+1234567890");
        phoneNumberResult.IsSuccess.Should().BeTrue();
        var phoneNumber = phoneNumberResult.Value;

        var patientResult = Patient.Create(name, gender, dateOfBirth, phoneNumber);
        patientResult.IsSuccess.Should().BeTrue();
        return patientResult.Value;
    }
}
