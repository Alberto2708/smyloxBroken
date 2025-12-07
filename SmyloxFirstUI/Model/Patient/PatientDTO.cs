using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Model.Patient
{
    public class PatientDTO
    {
        public Guid patientId { get; set; }
        public Guid doctorId { get; set; }
        public string doctorFirstName { get; set; }
        public string doctorLastName { get; set; }
        public string doctorEmail { get; set; }
        public string doctorUsername { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime createdAt { get; set; }

    }
}
