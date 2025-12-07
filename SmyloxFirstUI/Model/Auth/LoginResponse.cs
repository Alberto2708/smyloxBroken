using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Model.Auth
{
    public class LoginResponse
    {
        public string token { get; set; }
        public long expiresIn { get; set; }
        public Guid userId { get; set; }
        public string role { get; set; }

    }
}
