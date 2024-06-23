using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class UserLog
{
    public long UserLogId { get; set; }

    public long? UserId { get; set; }

    public string? Logtype { get; set; }

    public string? LogContent { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsDelete { get; set; }

    public virtual UserInfo? User { get; set; }
}
