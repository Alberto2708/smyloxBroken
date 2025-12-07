using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmyloxFirstUI.Model.Patient;

namespace SmyloxFirstUI.Helpers
{
    public class PatientService
    {
        private readonly ApiClient _apiClient;
        public PatientService(ApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<List<PatientDTO>> GetPatientsbyDoctorIdAsync(Guid doctorId)
        {
            var patients = await _apiClient.GetAsync<List<PatientDTO>>($"/patient/patients/{doctorId}");
            return patients;
        }

        public async Task<PatientDTO> GetPatientByIdAsync(Guid patientId)
        {
            var patient = await _apiClient.GetAsync<PatientDTO>($"/patient/{patientId}");
            return patient;
        }

        public async Task<PatientDTO> PostCreatePatient(CreatePatient patient)
        {
            var patientCreated = await _apiClient.PostAsync<PatientDTO>($"/patient/", patient);
            return patientCreated;
        }


    }
}
