using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Model.Patient
{
    public class CreatePatient
    {

        required public string firstName { get; set; }
        public string lastName { get; set; }

        required public Guid doctor { get; set; }

    }
}
