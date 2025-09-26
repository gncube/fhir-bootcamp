// src/PatientBridge.Core/Application/Patients/Queries/SearchPatientsQuery.cs
namespace PatientBridge.Core.Application.Patients.Queries;

public record SearchPatientsQuery(string SearchTerm, bool IncludePhoneSearch = false);
