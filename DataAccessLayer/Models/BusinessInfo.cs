using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class BusinessInfo
{
    public long BusinessInfoId { get; set; }

    public string? BusinessName { get; set; }

    public string? BusinessContactNumber { get; set; }

    public string? BusinessType { get; set; }

    public long? UserId { get; set; }

    public long? CountryId { get; set; }

    public long? StateId { get; set; }

    public long? CityId { get; set; }

    public string? ZipCode { get; set; }

    public string? LocationName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual City? City { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<KycInfo> KycInfos { get; set; } = new List<KycInfo>();

    public virtual State? State { get; set; }

    public virtual UserInfo? User { get; set; }
}
