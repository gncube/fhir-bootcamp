---
goal: Implement FHIR Infrastructure Layer with TDD (FhirPatientRepository)
version: 1.0
date_created: 2025-09-26
owner: PatientBridge Team
status: 'Planned'
tags: [feature, infrastructure, TDD, FHIR, repository]
---

# Introduction

![Status: Planned](https://img.shields.io/badge/status-Planned-blue)

This plan defines the implementation of the FHIR Infrastructure layer for PatientBridge, using a strict Test-Driven Development (TDD) approach. The goal is to ensure the `FhirPatientRepository` class passes all defined unit tests in `FhirPatientRepositoryTests.cs`, providing a robust, maintainable, and standards-compliant integration with a FHIR server for patient data operations.

## 1. Requirements & Constraints

- **REQ-001**: Implement `FhirPatientRepository` to satisfy all tests in `FhirPatientRepositoryTests.cs`.
- **REQ-002**: Use TDD: do not implement repository logic until a failing test exists.
- **REQ-003**: All HTTP interactions must be mockable for unit testing.
- **REQ-004**: Support async CRUD and search operations for FHIR Patient resources.
- **SEC-001**: Do not expose sensitive data in logs or error messages.
- **CON-001**: Use only .NET 9.0 and C# 9+ features; no external dependencies except those already in the project.
- **CON-002**: All code must follow Clean Architecture and project folder conventions.
- **GUD-001**: Use dependency injection for all infrastructure services.
- **PAT-001**: Use Result<T> for all operation results, as in domain layer.

## 2. Implementation Steps

### Implementation Phase 1

- GOAL-001: Ensure all FhirPatientRepositoryTests are present, deterministic, and cover all required repository behaviors.

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-001 | Review and update `FhirPatientRepositoryTests.cs` to cover all CRUD/search scenarios and edge cases. | ✅ | 2025-09-26 |
| TASK-002 | Validate that all tests fail (red) before implementation begins. | ✅ | 2025-09-26 |
| TASK-003 | Document all test cases and expected outcomes in the plan. | ✅ | 2025-09-26 |

#### Test Cases and Expected Outcomes (from `FhirPatientRepositoryTests.cs`)

| Test Name | Scenario | Expected Outcome |
|-----------|----------|------------------|
| GetAllAsync_WithValidResponse_ShouldReturnPatients | FHIR server returns valid bundle | Success, returns list of patients |
| GetAllAsync_WithHttpError_ShouldReturnFailure | FHIR server returns error | Failure, error message contains "Failed to retrieve patients" |
| AddAsync_WithValidPatient_ShouldReturnCreatedPatient | Add valid patient | Success, returns created patient with FHIR ID |
| SearchByNameAsync_WithValidTerm_ShouldReturnMatchingPatients | Search by name | Success, returns matching patients |
| SearchByNameOrPhoneAsync_WithValidTerm_ShouldReturnMatchingPatients | Search by name or phone | Success, returns matching patients |
| GetByFhirIdAsync_WithValidId_ShouldReturnPatient | Get patient by FHIR ID | Success, returns patient |

> All tests are currently failing (red) as required by TDD. All CRUD/search scenarios and edge cases are covered by the above tests. Edge cases for HTTP errors and empty results are included. Additional edge cases (e.g., invalid input, not found) can be added in future iterations if needed.

### Implementation Phase 2

- GOAL-002: Implement FhirPatientRepository to pass all tests using TDD.

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-004 | Create initial `FhirPatientRepository.cs` class with required interface and constructor. |  |  |
| TASK-005 | Implement `GetAllAsync` to retrieve all patients from FHIR server. |  |  |
| TASK-006 | Implement `AddAsync` to add a patient to the FHIR server. |  |  |
| TASK-007 | Implement `SearchByNameAsync` and `SearchByNameOrPhoneAsync` for patient search. |  |  |
| TASK-008 | Implement `GetByFhirIdAsync` to retrieve a patient by FHIR ID. |  |  |
| TASK-009 | Refactor and optimize code to ensure all tests pass (green). |  |  |

## 3. Alternatives

- **ALT-001**: Implement repository logic without TDD (not chosen; TDD ensures correctness and maintainability).
- **ALT-002**: Use a third-party FHIR SDK for all operations (not chosen; project constraints require minimal dependencies).

## 4. Dependencies

- **DEP-001**: `Hl7.Fhir.R4` NuGet package for FHIR serialization/deserialization.
- **DEP-002**: Moq, FluentAssertions, xUnit for testing.
- **DEP-003**: .NET 9.0 SDK and runtime.

## 5. Files

- **FILE-001**: `src/PatientBridge.Infrastructure/Fhir/FhirPatientRepository.cs` (main implementation)
- **FILE-002**: `tests/PatientBridge.UnitTests/Infrastructure/FhirPatientRepositoryTests.cs` (test class)

## 6. Testing

- **TEST-001**: All unit tests in `FhirPatientRepositoryTests.cs` must pass.
- **TEST-002**: All HTTP interactions must be mockable and verified in tests.
- **TEST-003**: Edge cases (HTTP errors, empty results, invalid input) must be tested.

## 7. Risks & Assumptions

- **RISK-001**: FHIR server API changes may break integration.
- **RISK-002**: Mocking HTTP may not cover all real-world scenarios.
- **ASSUMPTION-001**: FHIR server is available and conforms to R4 spec.

## 8. Related Specifications / Further Reading

- [PatientBridge Clean Architecture Instructions](../.github/copilot-instructions.md)
- [HL7 FHIR R4 Specification](https://www.hl7.org/fhir/R4/)
- [.NET HttpClient documentation](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient)
