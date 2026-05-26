namespace BSuit.Infrastructure.ExternalAPIs
{
    public interface IApiAuthClient
    {
        Task<string> GetTokenAsync(string username, string password);

        Task<TResponse> GetAsync<TResponse>(string url);

        Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data);

        Task<List<TResponse>> PostListAsync<TRequest, TResponse>(string url, List<TRequest> data);
    }
}
