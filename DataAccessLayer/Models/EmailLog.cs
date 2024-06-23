using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class EmailLog
{
    public long EmailLogSk { get; set; }

    public string? ContentType { get; set; }

    public string? From { get; set; }

    public string? To { get; set; }

    public string? Cc { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }

    public DateTime? SendOn { get; set; }

    public decimal? UserSk { get; set; }

    public bool? IsSend { get; set; }
}
