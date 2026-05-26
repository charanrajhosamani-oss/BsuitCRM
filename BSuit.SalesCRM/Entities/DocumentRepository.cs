using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class DocumentRepository
{
    public Guid AttachmentId { get; set; }

    public string ReferenceType { get; set; } = null!;

    public Guid ReferenceId { get; set; }

    public string? DocumentType { get; set; }

    public string FileName { get; set; } = null!;

    public string? OriginalFileName { get; set; }

    public string FilePath { get; set; } = null!;

    public string? FileExtension { get; set; }

    public long? FileSize { get; set; }

    public string? ContentType { get; set; }

    public string? Remarks { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
