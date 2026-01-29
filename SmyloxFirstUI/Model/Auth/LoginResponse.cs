using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Model.Auth
{
    public class LoginResponse
    {

        public string accessToken { get; set; } = string.Empty;
        public string refreshToken { get; set; } = string.Empty;

        public long expiresIn { get; set; }
        public Guid userId { get; set; }
        public string role { get; set; } = string.Empty;
        public string stripeCustomerId { get; set; } = string.Empty;

        public bool emailVerified { get; set; }





    }
}
