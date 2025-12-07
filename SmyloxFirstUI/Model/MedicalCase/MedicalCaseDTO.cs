using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Model.MedicalCase
{
    public class MedicalCaseDTO
    {
        public long id { get; set; }
        public Guid patientId { get; set; }
        public string patientFirstName { get; set; }
        public string patientLastName { get; set; }
        public Guid doctorId { get; set; }
        public string doctorFirstName { get; set; }
        public string doctorLastName { get; set; }
        public string doctorEmail { get; set; }
        public string doctorUsername { get; set; }
        public Guid? assistantId { get; set; }
        public string? assistantFirstName { get; set; }
        public string? assistantLastName { get; set; }
        public string? assistantEmail { get; set; }
        public string? assistantUsername { get; set; }
        public string category { get; set; }
        public string? diagnostic { get; set; }
        public DateTime lastModified { get; set; }
    }
}
