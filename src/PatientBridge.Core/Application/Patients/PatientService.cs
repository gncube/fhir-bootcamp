// src/PatientBridge.Core/Application/Patients/PatientService.cs
using PatientBridge.Core.Application.Patients.Commands;
using PatientBridge.Core.Application.Patients.DTOs;
using PatientBridge.Core.Application.Patients.Queries;
using PatientBridge.Core.Common;
using PatientBridge.Core.Domain.Patients;
using PatientBridge.Core.Domain.Patients.ValueObjects;

namespace PatientBridge.Core.Application.Patients;

public class PatientService
{
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<Result<PatientDto>> CreatePatientAsync(CreatePatientCommand command)
    {
        // Create value objects
        var nameResult = PatientName.Create(command.FirstName, command.LastName);
        if (nameResult.IsFailure)
            return Result.Failure<PatientDto>(nameResult.Error);

        var phoneResult = PhoneNumber.Create(command.PhoneNumber);
        if (phoneResult.IsFailure)
            return Result.Failure<PatientDto>(phoneResult.Error);

        // Create domain entity
        var patientResult = Patient.Create(
            nameResult.Value,
            command.Gender,
            command.DateOfBirth,
            phoneResult.Value
        );

        if (patientResult.IsFailure)
            return Result.Failure<PatientDto>(patientResult.Error);

        // Persist to repository
        var addResult = await _patientRepository.AddAsync(patientResult.Value);
        if (addResult.IsFailure)
            return Result.Failure<PatientDto>(addResult.Error);

        // Return DTO
        return Result.Success(PatientDto.FromDomain(addResult.Value));
    }

    public async Task<Result<IEnumerable<PatientDto>>> GetAllPatientsAsync()
    {
        var result = await _patientRepository.GetAllAsync();

        if (result.IsFailure)
            return Result.Failure<IEnumerable<PatientDto>>(result.Error);

        var dtos = result.Value.Select(PatientDto.FromDomain);
        return Result.Success(dtos);
    }

    public async Task<Result<PatientDto?>> GetPatientByIdAsync(Guid id)
    {
        var result = await _patientRepository.GetByIdAsync(id);

        if (result.IsFailure)
            return Result.Failure<PatientDto?>(result.Error);

        var dto = result.Value != null ? PatientDto.FromDomain(result.Value) : null;
        return Result.Success(dto);
    }

    public async Task<Result<IEnumerable<PatientDto>>> SearchPatientsAsync(SearchPatientsQuery query)
    {
        Result<IEnumerable<Patient>> result;

        if (query.IncludePhoneSearch)
        {
            result = await _patientRepository.SearchByNameOrPhoneAsync(query.SearchTerm);
        }
        else
        {
            result = await _patientRepository.SearchByNameAsync(query.SearchTerm);
        }

        if (result.IsFailure)
            return Result.Failure<IEnumerable<PatientDto>>(result.Error);

        var dtos = result.Value.Select(PatientDto.FromDomain);
        return Result.Success(dtos);
    }

    public async Task<Result> UpdatePatientAsync(UpdatePatientCommand command)
    {
        // Get existing patient
        var existingResult = await _patientRepository.GetByIdAsync(command.Id);
        if (existingResult.IsFailure)
            return Result.Failure(existingResult.Error);

        if (existingResult.Value == null)
            return Result.Failure("Patient not found");

        var patient = existingResult.Value;

        // Create value objects
        var nameResult = PatientName.Create(command.FirstName, command.LastName);
        if (nameResult.IsFailure)
            return Result.Failure(nameResult.Error);

        var phoneResult = PhoneNumber.Create(command.PhoneNumber);
        if (phoneResult.IsFailure)
            return Result.Failure(phoneResult.Error);

        // Update patient
        var updateResult = patient.UpdateDetails(
            nameResult.Value,
            command.Gender,
            command.DateOfBirth,
            phoneResult.Value
        );

        if (updateResult.IsFailure)
            return Result.Failure(updateResult.Error);

        // Persist changes
        return await _patientRepository.UpdateAsync(patient);
    }

    public async Task<Result> DeletePatientAsync(Guid id)
    {
        // Check if patient exists
        var existingResult = await _patientRepository.GetByIdAsync(id);
        if (existingResult.IsFailure)
            return Result.Failure(existingResult.Error);

        if (existingResult.Value == null)
            return Result.Failure("Patient not found");

        return await _patientRepository.DeleteAsync(id);
    }
}
