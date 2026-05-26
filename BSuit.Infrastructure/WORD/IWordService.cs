using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.Infrastructure.WORD
{
    public interface IWordService
    {
        byte[] GenerateFromHtml(string html);
        byte[] GenerateFromList<T>(IEnumerable<T> data, string title);


        Task<byte[]> GenerateFromViewAsync(string viewPath, object model);
    }
}
