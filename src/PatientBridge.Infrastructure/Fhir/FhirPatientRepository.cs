using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using PatientBridge.Core.Common;
using PatientBridge.Core.Domain.Patients;

namespace PatientBridge.Infrastructure.Fhir;

public class FhirPatientRepository : IPatientRepository
{
    private readonly HttpClient _httpClient;

    public FhirPatientRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<Result<IEnumerable<Patient>>> GetAllAsync()
        => throw new NotImplementedException();

    public Task<Result<Patient?>> GetByIdAsync(Guid id)
        => throw new NotImplementedException();

    public Task<Result<Patient?>> GetByFhirIdAsync(string fhirId)
        => throw new NotImplementedException();

    public Task<Result<IEnumerable<Patient>>> SearchByNameAsync(string searchTerm)
        => throw new NotImplementedException();

    public Task<Result<IEnumerable<Patient>>> SearchByNameOrPhoneAsync(string searchTerm)
        => throw new NotImplementedException();

    public Task<Result<Patient>> AddAsync(Patient patient)
        => throw new NotImplementedException();

    public Task<Result> UpdateAsync(Patient patient)
        => throw new NotImplementedException();

    public Task<Result> DeleteAsync(Guid id)
        => throw new NotImplementedException();
}
