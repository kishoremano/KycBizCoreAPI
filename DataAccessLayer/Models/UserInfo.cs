using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class UserInfo
{
    public long UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsActive { get; set; }

    public string? ContactNumber { get; set; }

    public string? ProfileUrl { get; set; }

    public string? UserType { get; set; }

    public string? ProfileContent { get; set; }

    public string? ProfileFileType { get; set; }

    public virtual ICollection<BusinessInfo> BusinessInfos { get; set; } = new List<BusinessInfo>();

    public virtual ICollection<KycInfoDoc> KycInfoDocs { get; set; } = new List<KycInfoDoc>();

    public virtual ICollection<KycInfo> KycInfos { get; set; } = new List<KycInfo>();

    public virtual ICollection<OtpLog> OtpLogs { get; set; } = new List<OtpLog>();

    public virtual ICollection<UserLog> UserLogs { get; set; } = new List<UserLog>();

    public virtual ICollection<UserMerchantLog> UserMerchantLogMerchantUsers { get; set; } = new List<UserMerchantLog>();

    public virtual ICollection<UserMerchantLog> UserMerchantLogUsers { get; set; } = new List<UserMerchantLog>();
}
