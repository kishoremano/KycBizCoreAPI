using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class KycInfoDoc
{
    public long KycInfoDocId { get; set; }

    public long? UserId { get; set; }

    public string? DocNameDesc { get; set; }

    public string? DocFileExn { get; set; }

    public long? DocSize { get; set; }

    public string? DocContent { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? KycInfoId { get; set; }

    public virtual KycInfo? KycInfo { get; set; }

    public virtual UserInfo? User { get; set; }
}
