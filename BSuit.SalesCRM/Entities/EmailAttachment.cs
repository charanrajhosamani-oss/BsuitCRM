using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class EmailAttachment
{
    public Guid AttachmentId { get; set; }

    public Guid EmailId { get; set; }

    public string FileName { get; set; } = null!;

    public string? FilePath { get; set; }

    public byte[]? FileData { get; set; }

    public string? MimeType { get; set; }

    public DateTime? CreatedOn { get; set; }

    public virtual EmailMessage Email { get; set; } = null!;
}
