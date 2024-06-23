using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class OtpLog
{
    public long Otpid { get; set; }

    public long? OtpCode { get; set; }

    public long? UserId { get; set; }

    public DateTime? OtpIssued { get; set; }

    public DateTime? OtpExpired { get; set; }

    public virtual UserInfo? User { get; set; }
}
