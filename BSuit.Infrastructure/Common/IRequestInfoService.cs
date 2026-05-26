using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.Infrastructure.Common
{
    public interface IRequestInfoService
    {
        string GetIpAddress();
        string GetBrowser();
    }
}
