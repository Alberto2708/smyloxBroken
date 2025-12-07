using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Helpers
{
    public class SessionService
    {

        public string Token { get; private set; } = string.Empty;
        public Guid UserId { get; private set; } = Guid.Empty;
        public long Expiry { get; private set; } = 0;
        public string Role { get; private set; } = string.Empty;

        public void Intitialize(string token, Guid userId, long expiry, string role)
        {
            Token = token;
            UserId = userId;
            Expiry = expiry;
            Role = role;
        }
    }
}
