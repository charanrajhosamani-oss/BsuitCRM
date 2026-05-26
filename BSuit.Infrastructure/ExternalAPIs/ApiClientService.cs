using System.Text;
using System.Text.Json;

namespace BSuit.Infrastructure.ExternalAPIs
{
    public class ApiClientService : IApiClientService
    {
        private readonly HttpClient _http;

        public ApiClientService(HttpClient http)
        {
            _http = http;
        }

        // ✅ GET single
        public async Task<TResponse> GetAsync<TResponse>(string url)
        {
            var response = await _http.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // ✅ GET list
        public async Task<List<TResponse>> GetListAsync<TResponse>(string url)
        {
            var response = await _http.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<TResponse>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // ✅ POST single
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var json = JsonSerializer.Serialize(data);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TResponse>(result,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // ✅ POST list
        public async Task<List<TResponse>> PostListAsync<TRequest, TResponse>(string url, List<TRequest> data)
        {
            var json = JsonSerializer.Serialize(data);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<TResponse>>(result,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }




    }
}
