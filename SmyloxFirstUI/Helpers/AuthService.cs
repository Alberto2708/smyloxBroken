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
        private readonly SecureTokenStorage _tokenStorage;

        public AuthService(ApiClient api, SessionService sessionService, SecureTokenStorage tokenStorage)
        {
            _api = api;
            _sessionService = sessionService;
            _tokenStorage = tokenStorage;
        }

        
        public async Task<bool> RefreshTokenAsync()
        {
            if (_api.GetIsRefreshing()) return false;

            try
            {
                _api.SetIsRefreshing(true);

                string refreshToken = _tokenStorage.retrieveToken("RefreshToken") ?? string.Empty;
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return false;
                }

                _api.RemoveTokenHeader();

                var refreshRequest = new { token = refreshToken };

                var response = await _api.PostAsync<RefreshTokenResponse?>("/auth/refresh", refreshRequest);

                if (response != null)
                {
                    if (!string.IsNullOrEmpty(response.access_token))
                    {
                        _tokenStorage.saveToken("AccessToken", response.access_token);
                        return true;
                    }

                }

                Logout();
                return false;
        

            } catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing token: {e.Message}");
                return false;
            }
            finally
            {
                _api.SetIsRefreshing(false);
            }
        }



        public async Task<LoginResponse?> Login(string _email, string _password)
        {
            var response = await _api.PostAsync<LoginResponse?>("/auth/login", new { email = _email, password = _password });

            if (response == null) return null;

            _sessionService.Intitialize(response.userId, response.expiresIn, response.role, response.stripeCustomerId, response.emailVerified);

            if (!string.IsNullOrEmpty(response.accessToken))
            {
                _tokenStorage.saveToken("AccessToken", response.accessToken);
            }

            if (!string.IsNullOrEmpty(response.refreshToken))
            {
                _tokenStorage.saveToken("RefreshToken", response.refreshToken);
            }

            return response;

        }

        public bool IsAuthenticated()
        {
            string accessToken = _tokenStorage.retrieveToken("AccessToken") ?? string.Empty;
            return !string.IsNullOrEmpty(accessToken);
        }

        public async Task<bool> RestoreSessionAsync()
        {
            try
            {
                if (!IsAuthenticated()) return false;

                bool isValid = await _api.ValidateTokenAsync();

                if(!isValid)
                {
                    bool refreshed = await RefreshTokenAsync();
                    if (!refreshed)
                    {
                        Logout();
                        return false;
                    }

                }

                return true;
            }
            catch
            {
                Logout();
                return false;
            }
        }



        private void Logout()
        {
            _tokenStorage.clearTokens();
            _api.RemoveTokenHeader();
            _sessionService.ClearSession();
        }


    }
}
