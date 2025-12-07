using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Model.Doctor
{
    public class DoctorDTO
    {
        public Guid doctorId { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public DateTime createdAt { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}
