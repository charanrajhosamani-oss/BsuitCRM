namespace BSuit.API.Models
{
    public class AlertMessage
    {
        public string? Type { get; set; }   // success, error, warning, info
        public string? Message { get; set; }
    }
}
