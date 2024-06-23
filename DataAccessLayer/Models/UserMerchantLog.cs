using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class UserMerchantLog
{
    public long UserMerchantLogId { get; set; }

    public long? UserId { get; set; }

    public long? MerchantUserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? ConnectedStatus { get; set; }

    public virtual UserInfo? MerchantUser { get; set; }

    public virtual UserInfo? User { get; set; }
}
