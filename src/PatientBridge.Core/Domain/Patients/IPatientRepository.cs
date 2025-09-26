// src/PatientBridge.Core/Domain/Patients/IPatientRepository.cs
using PatientBridge.Core.Common;

namespace PatientBridge.Core.Domain.Patients;

public interface IPatientRepository
{
    Task<Result<IEnumerable<Patient>>> GetAllAsync();
    Task<Result<Patient?>> GetByIdAsync(Guid id);
    Task<Result<Patient?>> GetByFhirIdAsync(string fhirId);
    Task<Result<IEnumerable<Patient>>> SearchByNameAsync(string searchTerm);
    Task<Result<IEnumerable<Patient>>> SearchByNameOrPhoneAsync(string searchTerm);
    Task<Result<Patient>> AddAsync(Patient patient);
    Task<Result> UpdateAsync(Patient patient);
    Task<Result> DeleteAsync(Guid id);
}
