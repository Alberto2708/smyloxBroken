using SmyloxFirstUI.Model.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmyloxFirstUI.Helpers;
using System.Diagnostics;

namespace SmyloxFirstUI.Stores
{
    public class DoctorStore
    {
        private readonly DoctorService _doctorService;
        private readonly SessionService _sessionService;
        private Lazy<Task> _initializeLazy;
        private DoctorDTO _doctor;

        public DoctorDTO Doctor => _doctor;

        public DoctorStore(DoctorService doctorService, SessionService sessionService)
        {
            _doctor = new DoctorDTO();
            _initializeLazy = new Lazy<Task>(Initialize);
            _doctorService = doctorService;
            _sessionService = sessionService;
        }

        private async Task Initialize()
        {
            var doctorData = await _doctorService.GetDoctorInfoByDoctorId(_sessionService.UserId);
            _doctor = doctorData;
            Debug.WriteLine($"DoctorStore initialized with Doctor ID: {_doctor.firstName}");

        }

        public async Task Load()
        {
            try
            {
                await _initializeLazy.Value;
            }
            catch (Exception)
            {
                _initializeLazy = new Lazy<Task>(Initialize);
                throw;
            }
        }
    }
}
