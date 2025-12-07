using SmyloxFirstUI.Model.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Helpers
{
    public class AuthService
    {
        private readonly ApiClient _api;
        private readonly SessionService _sessionService;

        public AuthService(ApiClient api, SessionService sessionService)
        {
            _api = api;
            _sessionService = sessionService;
        }

        public async Task<LoginResponse?> Login(string _email, string _password)
        {
            var response = await _api.PostAsync<LoginResponse?>("/auth/login", new { email = _email, password = _password });

            if (response == null) return null;

            _sessionService.Intitialize(response.token, response.userId, response.expiresIn, response.role);


            return response;

        }
    }
}
