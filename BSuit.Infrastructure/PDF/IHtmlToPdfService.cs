using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.Infrastructure.PDF
{
    public interface IHtmlToPdfService
    {
        byte[] GeneratePdf(string html);
    }

   
}
