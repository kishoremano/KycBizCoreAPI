using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class KycInfo
{
    public long KycInfoId { get; set; }

    public long? KycDocTypeId { get; set; }

    public string? KycNumber { get; set; }

    public long? UserId { get; set; }

    public DateTime? KycExpireDate { get; set; }

    public string? KycDocUrl { get; set; }

    public bool? IsActive { get; set; }

    public long? BusinessInfoId { get; set; }

    public virtual BusinessInfo? BusinessInfo { get; set; }

    public virtual KycDocType? KycDocType { get; set; }

    public virtual ICollection<KycInfoDoc> KycInfoDocs { get; set; } = new List<KycInfoDoc>();

    public virtual UserInfo? User { get; set; }
}
