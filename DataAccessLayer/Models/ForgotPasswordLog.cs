using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class ForgotPasswordLog
{
    public long ForgotPasswordid { get; set; }

    public string? Token { get; set; }

    public long? UserId { get; set; }

    public DateTime? TokenIssued { get; set; }

    public DateTime? TokenExpired { get; set; }

    public int? RequestCount { get; set; }

    public DateTime? LastPasswordResetRequest { get; set; }

    public bool? IsTokenVerified { get; set; }

    public string? Location { get; set; }

    public string? Device { get; set; }

    public bool? IsDeviceFound { get; set; }

    public bool? IsLocationFound { get; set; }
}
