
namespace BSuit.Infrastructure.ExternalAPIs
{
    public interface IApiClientService
    {
        Task<TResponse> GetAsync<TResponse>(string url);

        Task<List<TResponse>> GetListAsync<TResponse>(string url);

        Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data);

        Task<List<TResponse>> PostListAsync<TRequest, TResponse>(string url, List<TRequest> data);
    }
}
