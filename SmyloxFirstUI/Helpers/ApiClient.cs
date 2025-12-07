using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Helpers
{
    public class ApiClient
    {

        private readonly HttpClient _http;
        private readonly SessionService _sessionService;

        public ApiClient(HttpClient http, SessionService sessionService)
        {

            _http = http ?? throw new ArgumentNullException(nameof(http));
            _http.BaseAddress = new Uri("http://localhost:8081/");
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        private void AddTokenHeader()
        {
            if (!string.IsNullOrEmpty(_sessionService.Token))
            {
                if (_http.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _http.DefaultRequestHeaders.Remove("Authorization");
                }
                _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_sessionService.Token}");
            }

        }

        public async Task<T> GetAsync<T>(string url)
        {

            try
            {
                AddTokenHeader();
                var response = await _http.GetAsync(url);
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
                AddTokenHeader();
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _http.PostAsync(url, content);
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
                AddTokenHeader();
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _http.PutAsync(url, content);
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
                AddTokenHeader();
                var response = await _http.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json)!;
            }

            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"Error in DELETE request to {url}: {ex.Message}", ex);
            }
        }
    }
}
