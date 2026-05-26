using System.Text;

namespace BSuit.Infrastructure.CSV
{
    

    public class CsvService : ICsvService
    {
        public byte[] GenerateCsv<T>(IEnumerable<T> data)
        {
            var sb = new StringBuilder();
            var props = typeof(T).GetProperties();

            sb.AppendLine(string.Join(",", props.Select(p => p.Name)));

            foreach (var item in data)
            {
                var values = props.Select(p => p.GetValue(item)?.ToString());
                sb.AppendLine(string.Join(",", values));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
