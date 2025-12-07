using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmyloxFirstUI.Model.Patient;
using SmyloxFirstUI.Helpers;
using System.Diagnostics;

namespace SmyloxFirstUI.Stores
{
    public class PatientStore
    {
        private readonly PatientService _patientService;
        private readonly SessionService _sessionService;
        private Lazy<Task> _initializeLazy;
        private List<PatientDTO> _patientList;

        private PatientDTO _selectedPatient;

        public PatientDTO SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                _selectedPatient = value;
                Debug.WriteLine($"[PatientStore] SelectedPatient set: {_selectedPatient?.patientId}");
            }
        }

        public IEnumerable<PatientDTO> PatientList => _patientList;
        public PatientStore(PatientService patientService, SessionService sessionService)
        {
            _patientList = new List<PatientDTO>();
            _initializeLazy = new Lazy<Task>(Initialize);

            _patientService = patientService;
            _sessionService = sessionService;

            Debug.WriteLine($"[PatientStore] Created. Initial patient count: {_patientList.Count}");
        }

        private async Task Initialize()
        {
            Debug.WriteLine("[PatientStore] Initialize called.");
            var patientData = await _patientService.GetPatientsbyDoctorIdAsync(_sessionService.UserId);

            _patientList = patientData ?? new List<PatientDTO>();
            Debug.WriteLine($"[PatientStore] Loaded patients. Count: {_patientList.Count}");
        }

        public async Task Load()
        {
            Debug.WriteLine("[PatientStore] Load called.");
            try
            {
                await _initializeLazy.Value;
                Debug.WriteLine("[PatientStore] Load completed successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PatientStore] Load failed: {ex}");
                _initializeLazy = new Lazy<Task>(Initialize);
                throw;
            }
        }
    }
}
