using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Model.MedicalCase
{
    public class CreateMedicalCase
    {
        required public Guid patientId { get; set; }
        required public Guid doctorId { get; set; }
        public Guid? assistantId { get; set; }
        required public string category { get; set; }
        public string diagnostic { get; set; }

    }
}
