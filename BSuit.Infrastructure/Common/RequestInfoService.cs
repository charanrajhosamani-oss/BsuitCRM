using Microsoft.AspNetCore.Http;

namespace BSuit.Infrastructure.Common
{
   

    public class RequestInfoService : IRequestInfoService
    {
        private readonly IHttpContextAccessor _http;

        public RequestInfoService(IHttpContextAccessor http)
        {
            _http = http;
        }

        public string GetIpAddress()
        {
            return _http.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        public string GetBrowser()
        {
            return _http.HttpContext?.Request?.Headers["User-Agent"].ToString();
        }
    }
}
