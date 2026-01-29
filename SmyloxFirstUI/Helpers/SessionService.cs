using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Helpers
{
    public class SessionService
    {
        public Guid UserId { get; private set; } = Guid.Empty;
        public long Expiry { get; private set; } = 0;
        public string Role { get; private set; } = string.Empty;

        public string StripeCustomerId { get; set; } = string.Empty;

        public bool EmailVerified { get; set; } = false;

        public void Intitialize(Guid userId, long expiry, string role, string stripeCustomerId, bool emailVerified)
        {
            UserId = userId;
            Expiry = expiry;
            Role = role;
            StripeCustomerId = stripeCustomerId;
            EmailVerified = emailVerified;

        }

        public void ClearSession()
        {
            UserId = Guid.Empty;
            Expiry = 0;
            Role = string.Empty;
            StripeCustomerId = string.Empty;
            EmailVerified = false;
        }
    }
}
