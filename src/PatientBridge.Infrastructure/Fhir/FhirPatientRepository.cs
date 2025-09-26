using System.Net.Http;
using System.Threading.Tasks;
using PatientBridge.Core.Domain.Patients;

namespace PatientBridge.Infrastructure.Fhir;

public class FhirPatientRepository : IPatientRepository
{
    private readonly HttpClient _httpClient;

    public FhirPatientRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // TODO: Implement interface methods using TDD
}
