using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmyloxFirstUI.Model.MedicalCase;
using SmyloxFirstUI.Helpers;
using SmyloxFirstUI.Model.Patient;
using System.Diagnostics;

namespace SmyloxFirstUI.Stores
{
    public class MedicalCaseStore
    {
        private readonly MedicalCaseService _medicalCaseService;
        private readonly PatientStore _patientStore;
        private List<MedicalCaseDTO> _medicalCaseList;

        public IEnumerable<MedicalCaseDTO> MedicalCaseList => _medicalCaseList;

        public MedicalCaseStore(MedicalCaseService medicalCaseService, SessionService sessionService, PatientStore patientStore)
        {
            _medicalCaseList = new List<MedicalCaseDTO>();

            _medicalCaseService = medicalCaseService;
            _patientStore = patientStore;

            Debug.WriteLine($"[MedicalCaseStore] Created. Initial medical case count: {_medicalCaseList.Count}");
        }

        private async Task Initialize()
        {
            Debug.WriteLine("[MedicalCaseStore] Initialize called.");

            if (_patientStore.SelectedPatient == null)
            {
                Debug.WriteLine("[MedicalCaseStore] No patient selected. Throwing InvalidOperationException.");
                throw new InvalidOperationException("No patient selected.");
            }

            Debug.WriteLine($"[MedicalCaseStore] SelectedPatient id: {_patientStore.SelectedPatient?.patientId}");

            var medicalCaseData = await _medicalCaseService
                .GetMedicalCasesByPatientIdAsync(_patientStore.SelectedPatient.patientId);

            _medicalCaseList = medicalCaseData ?? new List<MedicalCaseDTO>();

            Debug.WriteLine($"[MedicalCaseStore] Loaded medical cases. Count: {_medicalCaseList?.Count}");
        }

        public async Task Load()
        {
            Debug.WriteLine("[MedicalCaseStore] Load called.");
            try
            {
                await Initialize();
                Debug.WriteLine("[MedicalCaseStore] Load completed successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MedicalCaseStore] Load failed: {ex}");
                throw;
            }
        }

        public void ClearMedicalCases()
        {
            _medicalCaseList.Clear();
        }
    }
}
