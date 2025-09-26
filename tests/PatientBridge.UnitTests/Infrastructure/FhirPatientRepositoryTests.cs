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

    // Minimal test helpers for current scenarios
    private void SetupHttpResponse(HttpStatusCode statusCode, string content)
    {
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
    }

    private string CreateFhirPatientBundle(params Patient[] patients)
    {
        // Minimal valid FHIR bundle mock
        if (patients == null || patients.Length == 0)
            return "{\"resourceType\":\"Bundle\",\"entry\":[]}";

        var entries = patients.Select(p => $"{{\"resource\":{{\"resourceType\":\"Patient\",\"id\":\"{p.FhirId}\",\"name\":[{{\"given\":[\"{p.Name.FirstName}\"],\"family\":\"{p.Name.LastName}\"}}]}}}}")
            .ToArray();
        var entryJson = string.Join(",", entries);
        return $"{{\"resourceType\":\"Bundle\",\"entry\":[{entryJson}]}}";
    }

    private Patient CreateValidPatient()
    {
        var nameResult = PatientName.Create("John", "Doe");
        var phoneResult = PhoneNumber.Create("+15551234");
        var gender = Gender.Male; // Use Gender.Male as default
        var dob = DateOnly.Parse("1980-01-01");
        if (!nameResult.IsSuccess || !phoneResult.IsSuccess)
            throw new InvalidOperationException("Failed to create test patient name or phone");
        var patientResult = Patient.Create(nameResult.Value, gender, dob, phoneResult.Value);
        if (!patientResult.IsSuccess)
            throw new InvalidOperationException("Failed to create test patient");
        var patient = patientResult.Value;
        patient.SetFhirId("patient-1");
        return patient;
    }

    private string CreateFhirPatientResponse(Patient patient)
    {
        // Minimal valid FHIR Patient resource mock
        return $"{{\"resourceType\":\"Patient\",\"id\":\"{patient.FhirId}\",\"name\":[{{\"given\":[\"{patient.Name.FirstName}\"],\"family\":\"{patient.Name.LastName}\"}}]}}";
    }

    private void VerifyHttpRequest(HttpMethod method, string url)
    {
        _mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.AtLeastOnce(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == method && req.RequestUri!.ToString().Contains(url)),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task GetByFhirIdAsync_WithInvalidId_ShouldReturnFailure()
    {
        // Arrange
        var invalidFhirId = "!@#invalid";
        SetupHttpResponse(HttpStatusCode.BadRequest, "Bad Request");

        // Act
        var result = await _repository.GetByFhirIdAsync(invalidFhirId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Failed to retrieve patient");
    }

    [Fact]
    public async Task GetByFhirIdAsync_WithNotFound_ShouldReturnNull()
    {
        // Arrange
        var fhirId = "notfound-123";
        SetupHttpResponse(HttpStatusCode.NotFound, "Not Found");

        // Act
        var result = await _repository.GetByFhirIdAsync(fhirId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WithInvalidPatient_ShouldReturnFailure()
    {
        // Arrange
        var invalidPatient = (Patient)null!;
        // No HTTP call expected, but simulate a bad request if attempted
        SetupHttpResponse(HttpStatusCode.BadRequest, "Bad Request");

        // Act
        var result = await _repository.AddAsync(invalidPatient);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Invalid patient");
    }

    [Fact]
    public async Task SearchByNameAsync_WithNoResults_ShouldReturnEmptyList()
    {
        // Arrange
        var searchTerm = "Nonexistent";
        var emptyBundle = "{\"resourceType\":\"Bundle\",\"entry\":[]}";
        SetupHttpResponse(HttpStatusCode.OK, emptyBundle);

        // Act
        var result = await _repository.SearchByNameAsync(searchTerm);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
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
        // Simulate returned patient with FHIR ID "patient-123"
        var returnedPatient = CreateValidPatient();
        returnedPatient.SetFhirId("patient-123");
        var fhirPatientResponse = CreateFhirPatientResponse(returnedPatient);
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
        var returnedPatient = CreateValidPatient();
        returnedPatient.SetFhirId(fhirId);
        var fhirPatientResponse = CreateFhirPatientResponse(returnedPatient);
        SetupHttpResponse(HttpStatusCode.OK, fhirPatientResponse);

        // Act
        var result = await _repository.GetByFhirIdAsync(fhirId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().NotBeNull();

    }

}
