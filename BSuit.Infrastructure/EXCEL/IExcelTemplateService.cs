using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.Infrastructure.EXCEL
{
    public interface IExcelTemplateService
    {
        byte[] ExportFromTemplate<T>(IEnumerable<T> data, string templatePath);
    }
}
