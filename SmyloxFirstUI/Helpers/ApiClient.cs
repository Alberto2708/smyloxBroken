using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Helpers
{
    public class ApiClient
    {

        private readonly HttpClient _http;
        private readonly SecureTokenStorage _tokenStorage;
        private readonly SessionService _sessionService;
        private readonly AuthService _authService;
        private bool _isRefreshing = false;

        public ApiClient(HttpClient http, SessionService sessionService, SecureTokenStorage tokenStorage, AuthService authService)
        {

            _http = http ?? throw new ArgumentNullException(nameof(http));
            _http.BaseAddress = new Uri("http://localhost:8081/");
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _tokenStorage = tokenStorage;
            _authService = authService;
        }

        private void AddTokenHeader()
        {

            if (_http.DefaultRequestHeaders.Authorization == null) return;

            string token = _tokenStorage.retrieveToken("AccessToken") ?? string.Empty;

            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
            }

        }

        public void RemoveTokenHeader()
        {
            _http.DefaultRequestHeaders.Authorization = null;
        }


        public async Task<T> GetAsync<T>(string url)
        {

            try
            {

                var response = await ExecuteWithTokenRefreshAsync(() => _http.GetAsync(url));

                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json)!;
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"Error in GET request to {url}: {ex.Message}", ex);
            }
        }

        public async Task<T> PostAsync<T>(string url, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");


                var response = await ExecuteWithTokenRefreshAsync(() => _http.PostAsync(url, content));

                response.EnsureSuccessStatusCode();

                string responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseJson)!;
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"Error in POST request to {url}: {ex.Message}", ex);
            }
        }

        public async Task<T> PutAsync<T>(string url, object data)
        {
            try

            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await ExecuteWithTokenRefreshAsync(() => _http.PutAsync(url, content));
                response.EnsureSuccessStatusCode();

                string responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseJson)!;
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"Error in PUT request to {url}: {ex.Message}", ex);
            }
        }

        public async Task<T> DeleteAsync<T>(string url)
        {
            try
            {
                var response = await ExecuteWithTokenRefreshAsync(() => _http.DeleteAsync(url));
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json)!;
            }

            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"Error in DELETE request to {url}: {ex.Message}", ex);
            }
        }

        private async Task<HttpResponseMessage> ExecuteWithTokenRefreshAsync(Func<Task<HttpResponseMessage>> requestFunc)
        {

            AddTokenHeader();
            var response = await requestFunc();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshed = await _authService.RefreshTokenAsync();

                if (refreshed)
                {
                    AddTokenHeader();
                    response = await requestFunc();
                }

                else
                {
                    throw new UnauthorizedAccessException("Session expired. Please login again");
                }
            }

            return response;
        }

        public async Task<bool> ValidateTokenAsync()
        {
            try
            {
                string token = _tokenStorage.retrieveToken("AccessToken") ?? string.Empty;
                if (string.IsNullOrEmpty(token)) return false;

                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _http.GetAsync("auth/validate");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public bool GetIsRefreshing()
        {
            return _isRefreshing;
        }

        public void SetIsRefreshing(bool value)
        {
            _isRefreshing = value;
        }
    }
}
