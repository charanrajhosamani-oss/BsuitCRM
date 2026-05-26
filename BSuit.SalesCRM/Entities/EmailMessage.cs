using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class EmailMessage
{
    public Guid EmailId { get; set; }

    public string? Subject { get; set; }

    public string? BodyHtml { get; set; }

    public DateTime? SentDate { get; set; }

    public string Status { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public string ParentEntityType { get; set; } = null!;

    public Guid ParentEntityId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public virtual ICollection<EmailAttachment> EmailAttachments { get; set; } = new List<EmailAttachment>();

    public virtual ICollection<EmailRecipient> EmailRecipients { get; set; } = new List<EmailRecipient>();
}
