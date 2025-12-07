using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmyloxFirstUI.Model.Doctor;

namespace SmyloxFirstUI.Helpers
{
    public class DoctorService
    {
        private readonly ApiClient _apiClient;
        public DoctorService(ApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<DoctorDTO> GetDoctorInfoByDoctorId(Guid doctorId)
        {
            var doctor = await _apiClient.GetAsync<DoctorDTO>($"/doctor/{doctorId}");
            return doctor;
        }

    }
}
