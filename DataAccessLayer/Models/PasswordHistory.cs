using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class PasswordHistory
{
    public long PasswordHistoryId { get; set; }

    public string? Password { get; set; }

    public long? UserId { get; set; }

    public DateTime? CreatedDate { get; set; }
}
