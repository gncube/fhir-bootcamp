// tests/PatientBridge.UnitTests/Infrastructure/FhirPatientRepositoryTests.cs
using FluentAssertions;
using Moq;
using Moq.Protected;
using PatientBridge.Core.Domain.Patients;
using PatientBridge.Core.Domain.Patients.ValueObjects;
using PatientBridge.Infrastructure.Fhir;
using System.Net;
using System.Text;
using Xunit;

namespace PatientBridge.UnitTests.Infrastructure;

public class FhirPatientRepositoryTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;
    private readonly HttpClient _httpClient;
    private readonly FhirPatientRepository _repository;
    private const string BaseUrl = "https://fhir-bootcamp.medblocks.com/fhir";

    public FhirPatientRepositoryTests()
    {
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpHandler.Object)
        {
            BaseAddress = new Uri(BaseUrl)
        };
        _repository = new FhirPatientRepository(_httpClient);
    }

    [Fact]
    public async Task GetAllAsync_WithValidResponse_ShouldReturnPatients()
    {
        // Arrange
        var fhirBundle = CreateFhirPatientBundle();
        SetupHttpResponse(HttpStatusCode.OK, fhirBundle);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);

        var patients = result.Value.ToList();
        patients[0].Name.FirstName.Should().Be("John");
        patients[0].Name.LastName.Should().Be("Doe");
        patients[0].FhirId.Should().Be("patient-1");
    }

    [Fact]
    public async Task GetAllAsync_WithHttpError_ShouldReturnFailure()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.InternalServerError, "Server Error");

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Failed to retrieve patients");
    }

    [Fact]
    public async Task AddAsync_WithValidPatient_ShouldReturnCreatedPatient()
    {
        // Arrange
        var patient = CreateValidPatient();
        var fhirPatientResponse = CreateFhirPatientResponse("patient-123");
        SetupHttpResponse(HttpStatusCode.Created, fhirPatientResponse);

        // Act
        var result = await _repository.AddAsync(patient);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.FhirId.Should().Be("patient-123");

        // Verify POST request was made
        VerifyHttpRequest(HttpMethod.Post, "/Patient");
    }

    [Fact]
    public async Task SearchByNameAsync_WithValidTerm_ShouldReturnMatchingPatients()
    {
        // Arrange
        var searchTerm = "John";
        var fhirBundle = CreateFhirPatientBundle();
        SetupHttpResponse(HttpStatusCode.OK, fhirBundle);

        // Act
        var result = await _repository.SearchByNameAsync(searchTerm);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);

        // Verify search request was made with correct parameters
        VerifyHttpRequest(HttpMethod.Get, $"/Patient?name={searchTerm}");
    }

    [Fact]
    public async Task SearchByNameOrPhoneAsync_WithValidTerm_ShouldReturnMatchingPatients()
    {
        // Arrange
        var searchTerm = "123";
        var fhirBundle = CreateFhirPatientBundle();
        SetupHttpResponse(HttpStatusCode.OK, fhirBundle);

        // Act
        var result = await _repository.SearchByNameOrPhoneAsync(searchTerm);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByFhirIdAsync_WithValidId_ShouldReturnPatient()
    {
        // Arrange
        var fhirId = "patient-123";
        var fhirPatientResponse = CreateFhirPatientResponse(fhirId);
        SetupHttpResponse(HttpStatusCode.OK, fhirPatientResponse);

        // Act
        var result = await _repository.GetByFhirIdAsync(fhirId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().NotBeNull();

    }

}
