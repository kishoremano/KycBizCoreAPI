using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class KycDocType
{
    public long KycDocId { get; set; }

    public string? KycDocName { get; set; }

    public virtual ICollection<KycInfo> KycInfos { get; set; } = new List<KycInfo>();
}
