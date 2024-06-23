using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class MessageContentMaster
{
    public decimal MessageContentMasterSk { get; set; }

    public string? ContentType { get; set; }

    public string? EmailContent { get; set; }

    public string? SmsContent { get; set; }

    public decimal? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public decimal? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsActive { get; set; }
}
