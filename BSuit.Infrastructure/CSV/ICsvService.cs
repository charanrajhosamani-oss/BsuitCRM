using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.Infrastructure.CSV
{
    public interface ICsvService
    {
        byte[] GenerateCsv<T>(IEnumerable<T> data);
    }
}
