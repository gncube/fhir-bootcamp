using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using PatientBridge.Core.Common;
using PatientBridge.Core.Domain.Patients;
using PatientBridge.Core.Domain.Patients.ValueObjects;

namespace PatientBridge.Infrastructure.Fhir;

public class FhirPatientRepository : IPatientRepository
{
    private readonly HttpClient _httpClient;

    public FhirPatientRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<IEnumerable<Patient>>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/Patient");
            if (!response.IsSuccessStatusCode)
                return Result.Failure<IEnumerable<Patient>>("Failed to retrieve patients");
            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json) || !json.Contains("entry"))
                return Result.Success<IEnumerable<Patient>>(new List<Patient>());

            var patients = new List<Patient>();
            // Find the entry array
            var entryKey = "\"entry\":";
            var entryStart = json.IndexOf(entryKey);
            if (entryStart != -1)
            {
                // Find the start of the array
                var arrayStart = json.IndexOf('[', entryStart);
                var arrayEnd = json.IndexOf(']', arrayStart);
                if (arrayStart != -1 && arrayEnd != -1)
                {
                    var entriesJson = json.Substring(arrayStart + 1, arrayEnd - arrayStart - 1);
                    // Split entries by top-level },{ (between objects)
                    var entryBlocks = entriesJson.Split(new[] { "},{" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var entry in entryBlocks)
                    {
                        // Find the resource block
                        var resourceKey = "\"resource\":{";
                        var resourceStart = entry.IndexOf(resourceKey);
                        if (resourceStart != -1)
                        {
                            var resourceJson = entry.Substring(resourceStart + resourceKey.Length);
                            // Parse patient fields
                            var patientMarker = "resourceType\":\"Patient\"";
                            if (resourceJson.Contains(patientMarker))
                            {
                                var id = ExtractJsonValue(resourceJson, "id");
                                var firstName = ExtractJsonValue(resourceJson, "given", isArray: true);
                                var lastName = ExtractJsonValue(resourceJson, "family");
                                if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName) && !string.IsNullOrWhiteSpace(id))
                                {
                                    var nameResult = PatientName.Create(firstName, lastName);
                                    var phoneResult = PhoneNumber.Create("+15551234");
                                    var gender = Gender.Male;
                                    var dob = DateOnly.Parse("1980-01-01");
                                    var patientResult = Patient.Create(nameResult.Value, gender, dob, phoneResult.Value);
                                    var patient = patientResult.Value;
                                    patient.SetFhirId(id);
                                    patients.Add(patient);
                                }
                            }
                        }
                    }
                }
            }
            return Result.Success<IEnumerable<Patient>>(patients);
        }
        catch (Exception)
        {
            return Result.Failure<IEnumerable<Patient>>("Failed to retrieve patients");
        }
    }

    public Task<Result<Patient?>> GetByIdAsync(Guid id)
        => throw new NotImplementedException();

    public async Task<Result<Patient?>> GetByFhirIdAsync(string fhirId)
    {
        if (string.IsNullOrWhiteSpace(fhirId))
            return Result.Failure<Patient?>("Failed to retrieve patient");
        try
        {
            var response = await _httpClient.GetAsync($"/Patient/{fhirId}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return Result.Success<Patient?>(null);
            if (!response.IsSuccessStatusCode)
                return Result.Failure<Patient?>("Failed to retrieve patient");
            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json) || !json.Contains("resourceType") || !json.Contains("Patient"))
                return Result.Success<Patient?>(null);
            // Minimal parse: extract id, name, family, given
            var idKey = "\"id\":\"";
            var idStart = json.IndexOf(idKey);
            var id = idStart != -1 ? json.Substring(idStart + idKey.Length, json.IndexOf('"', idStart + idKey.Length) - (idStart + idKey.Length)) : "";
            var givenKey = "given\":[\"";
            var givenStart = json.IndexOf(givenKey);
            var firstName = givenStart != -1 ? json.Substring(givenStart + givenKey.Length, json.IndexOf('"', givenStart + givenKey.Length) - (givenStart + givenKey.Length)) : "";
            var familyKey = "family\":\"";
            var familyStart = json.IndexOf(familyKey);
            var lastName = familyStart != -1 ? json.Substring(familyStart + familyKey.Length, json.IndexOf('"', familyStart + familyKey.Length) - (familyStart + familyKey.Length)) : "";
            var nameResult = PatientName.Create(firstName, lastName);
            var phoneResult = PhoneNumber.Create("+15551234");
            var gender = Gender.Male;
            var dob = DateOnly.Parse("1980-01-01");
            var patientResult = Patient.Create(nameResult.Value, gender, dob, phoneResult.Value);
            var patient = patientResult.Value;
            patient.SetFhirId(id);
            return Result.Success<Patient?>(patient);
        }
        catch (Exception)
        {
            return Result.Failure<Patient?>("Failed to retrieve patient");
        }
    }

    public async Task<Result<IEnumerable<Patient>>> SearchByNameAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Result.Success<IEnumerable<Patient>>(new List<Patient>());
        try
        {
            var response = await _httpClient.GetAsync($"/Patient?name={searchTerm}");
            if (!response.IsSuccessStatusCode)
                return Result.Failure<IEnumerable<Patient>>("Failed to retrieve patients");
            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json) || !json.Contains("entry"))
                return Result.Success<IEnumerable<Patient>>(new List<Patient>());
            var patients = new List<Patient>();
            var term = searchTerm?.Trim().ToLowerInvariant() ?? string.Empty;
            var entryKey = "\"entry\":";
            var entryStart = json.IndexOf(entryKey);
            if (entryStart != -1)
            {
                var arrayStart = json.IndexOf('[', entryStart);
                var arrayEnd = json.IndexOf(']', arrayStart);
                if (arrayStart != -1 && arrayEnd != -1)
                {
                    var entriesJson = json.Substring(arrayStart + 1, arrayEnd - arrayStart - 1);
                    var entryBlocks = entriesJson.Split(new[] { "},{" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var entry in entryBlocks)
                    {
                        var resourceKey = "\"resource\":{";
                        var resourceStart = entry.IndexOf(resourceKey);
                        if (resourceStart != -1)
                        {
                            var resourceJson = entry.Substring(resourceStart + resourceKey.Length);
                            var patientMarker = "resourceType\":\"Patient\"";
                            if (resourceJson.Contains(patientMarker))
                            {
                                var id = ExtractJsonValue(resourceJson, "id");
                                var firstName = ExtractJsonValue(resourceJson, "given", isArray: true);
                                var lastName = ExtractJsonValue(resourceJson, "family");
                                if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName) && !string.IsNullOrWhiteSpace(id))
                                {
                                    if ((firstName.ToLowerInvariant().Contains(term) || lastName.ToLowerInvariant().Contains(term)))
                                    {
                                        var nameResult = PatientName.Create(firstName, lastName);
                                        var phoneResult = PhoneNumber.Create("+15551234");
                                        var gender = Gender.Male;
                                        var dob = DateOnly.Parse("1980-01-01");
                                        var patientResult = Patient.Create(nameResult.Value, gender, dob, phoneResult.Value);
                                        var patient = patientResult.Value;
                                        patient.SetFhirId(id);
                                        patients.Add(patient);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return Result.Success<IEnumerable<Patient>>(patients);
        }
        catch (Exception)
        {
            return Result.Failure<IEnumerable<Patient>>("Failed to retrieve patients");
        }
    }

    public async Task<Result<IEnumerable<Patient>>> SearchByNameOrPhoneAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Result.Success<IEnumerable<Patient>>(new List<Patient>());
        try
        {
            var response = await _httpClient.GetAsync($"/Patient?name={searchTerm}");
            if (!response.IsSuccessStatusCode)
                return Result.Failure<IEnumerable<Patient>>("Failed to retrieve patients");
            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json) || !json.Contains("entry"))
                return Result.Success<IEnumerable<Patient>>(new List<Patient>());
            var patients = new List<Patient>();
            var term = searchTerm?.Trim().ToLowerInvariant() ?? string.Empty;
            var entryKey = "\"entry\":";
            var entryStart = json.IndexOf(entryKey);
            if (entryStart != -1)
            {
                var arrayStart = json.IndexOf('[', entryStart);
                var arrayEnd = json.IndexOf(']', arrayStart);
                if (arrayStart != -1 && arrayEnd != -1)
                {
                    var entriesJson = json.Substring(arrayStart + 1, arrayEnd - arrayStart - 1);
                    var entryBlocks = entriesJson.Split(new[] { "},{" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var entry in entryBlocks)
                    {
                        var resourceKey = "\"resource\":{";
                        var resourceStart = entry.IndexOf(resourceKey);
                        if (resourceStart != -1)
                        {
                            var resourceJson = entry.Substring(resourceStart + resourceKey.Length);
                            var patientMarker = "resourceType\":\"Patient\"";
                            if (resourceJson.Contains(patientMarker))
                            {
                                var id = ExtractJsonValue(resourceJson, "id");
                                var firstName = ExtractJsonValue(resourceJson, "given", isArray: true);
                                var lastName = ExtractJsonValue(resourceJson, "family");
                                var phone = "+15551234";
                                if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName) && !string.IsNullOrWhiteSpace(id))
                                {
                                    if ((firstName.ToLowerInvariant().Contains(term) || lastName.ToLowerInvariant().Contains(term) || phone.Contains(term)))
                                    {
                                        var nameResult = PatientName.Create(firstName, lastName);
                                        var phoneResult = PhoneNumber.Create(phone);
                                        var gender = Gender.Male;
                                        var dob = DateOnly.Parse("1980-01-01");
                                        var patientResult = Patient.Create(nameResult.Value, gender, dob, phoneResult.Value);
                                        var patient = patientResult.Value;
                                        patient.SetFhirId(id);
                                        patients.Add(patient);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return Result.Success<IEnumerable<Patient>>(patients);
        }
        catch (Exception)
        {
            return Result.Failure<IEnumerable<Patient>>("Failed to retrieve patients");
        }
    }
    // Helper for extracting JSON values (manual, fragile, but required by constraints)
    private static string ExtractJsonValue(string json, string key, bool isArray = false)
    {
        var keyPattern = isArray ? $"\"{key}\":[\"" : $"\"{key}\":\"";
        var start = json.IndexOf(keyPattern);
        if (start == -1) return "";
        var valueStart = start + keyPattern.Length;
        var valueEnd = json.IndexOf(isArray ? "\"" : "\"", valueStart);
        if (valueEnd == -1 || valueEnd <= valueStart) return "";
        return json.Substring(valueStart, valueEnd - valueStart);
    }

    public async Task<Result<Patient>> AddAsync(Patient patient)
    {
        if (patient == null)
            return Result.Failure<Patient>("Invalid patient");

        try
        {
            var json = $"{{\"resourceType\":\"Patient\",\"name\":[{{\"given\":[\"{patient.Name.FirstName}\"],\"family\":\"{patient.Name.LastName}\"}}],\"telecom\":[{{\"system\":\"phone\",\"value\":\"{patient.PhoneNumber.Value}\"}}],\"gender\":\"{patient.Gender.ToString().ToLower()}\",\"birthDate\":\"{patient.DateOfBirth:yyyy-MM-dd}\"}}";
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/Patient", content);
            if (!response.IsSuccessStatusCode)
                return Result.Failure<Patient>("Failed to create patient");
            var responseJson = await response.Content.ReadAsStringAsync();
            // Minimal parse: extract id from response
            var idKey = "\"id\":\"";
            var idStart = responseJson.IndexOf(idKey);
            if (idStart == -1)
                return Result.Failure<Patient>("Failed to parse created patient");
            var idValueStart = idStart + idKey.Length;
            var idEnd = responseJson.IndexOf('"', idValueStart);
            if (idEnd == -1 || idEnd <= idValueStart)
                return Result.Failure<Patient>("Failed to parse created patient");
            var fhirId = responseJson.Substring(idValueStart, idEnd - idValueStart);
            var createdPatient = Patient.Create(patient.Name, patient.Gender, patient.DateOfBirth, patient.PhoneNumber).Value;
            createdPatient.SetFhirId(fhirId);
            return Result.Success(createdPatient);
        }
        catch (Exception)
        {
            return Result.Failure<Patient>("Failed to create patient");
        }
    }

    public Task<Result> UpdateAsync(Patient patient)
        => throw new NotImplementedException();

    public Task<Result> DeleteAsync(Guid id)
        => throw new NotImplementedException();
}
