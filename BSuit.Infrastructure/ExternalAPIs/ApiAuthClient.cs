using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace BSuit.Infrastructure.ExternalAPIs
{
    public class ApiAuthClient : IApiAuthClient
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContext;

        private string Token
        {
            get => _httpContext.HttpContext?.Session.GetString("ApiToken");
            set => _httpContext.HttpContext?.Session.SetString("ApiToken", value);
        }

        public ApiAuthClient(HttpClient http, IHttpContextAccessor httpContext)
        {
            _http = http;
            _httpContext = httpContext;
        }

        // 🔐 1. Get Token
        public async Task<string> GetTokenAsync(string username, string password)
        {
            var request = new
            {
                username,
                password
            };

            var json = JsonSerializer.Serialize(request);

            var response = await _http.PostAsync("/auth/login",
                new StringContent(json, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            var tokenObj = JsonSerializer.Deserialize<TokenResponse>(result,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Token = tokenObj.AccessToken;

            return Token;
        }

        // 🔧 Attach Token
        private void AttachToken()
        {
            if (!string.IsNullOrEmpty(Token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Token);
            }
        }

        // ✅ GET
        public async Task<TResponse> GetAsync<TResponse>(string url)
        {
            AttachToken();

            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // ✅ POST single
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            AttachToken();

            var json = JsonSerializer.Serialize(data);

            var response = await _http.PostAsync(url,
                new StringContent(json, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TResponse>(result,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // ✅ POST list
        public async Task<List<TResponse>> PostListAsync<TRequest, TResponse>(string url, List<TRequest> data)
        {
            AttachToken();

            var json = JsonSerializer.Serialize(data);

            var response = await _http.PostAsync(url,
                new StringContent(json, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<TResponse>>(result,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }

   
}
