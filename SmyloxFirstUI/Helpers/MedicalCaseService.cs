using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmyloxFirstUI.Model.MedicalCase;

namespace SmyloxFirstUI.Helpers
{
    public class MedicalCaseService
    {
        private readonly ApiClient _apiClient;
        public MedicalCaseService(ApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<List<MedicalCaseDTO>> GetMedicalCasesByPatientIdAsync(Guid patientId)
        {
            var medicalCases = await _apiClient.GetAsync<List<MedicalCaseDTO>>($"/case/patient/{patientId}");
            Debug.WriteLine($"Fetched {medicalCases.Count} medical cases for patient ID: {patientId}");
            return medicalCases;
        }

        public async Task<MedicalCaseDTO> PostCreateMedicalCase(CreateMedicalCase medicalCase)
        {
            var medicalCaseCreated = await _apiClient.PostAsync<MedicalCaseDTO>($"/case/", medicalCase);
            return medicalCaseCreated;
        }
    }
}
