using BSuit.SalesCRM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.SalesCRM.VM.Lead
{
    public class NDAAgreementSubmitVM
    {
        public Guid LeadId { get; set; }

        public string? RequestedBy { get; set; }

        public string? RequestedDate { get; set; }

        public bool AcceptedAgreement { get; set; }

        public bool AcceptScannedDocuments { get; set; }
    }

    public class NDAAgreementVM
    {
        public Guid LeadId { get; set; }

        public string? CustomerName { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        public string? RequestedBy { get; set; }

        public string? RequestedDate { get; set; }

        public NDASignature? NDAData { get; set; }
    }
}
