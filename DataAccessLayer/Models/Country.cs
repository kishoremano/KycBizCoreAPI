using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Country
{
    public long CountryId { get; set; }

    public string? CountryName { get; set; }

    public virtual ICollection<BusinessInfo> BusinessInfos { get; set; } = new List<BusinessInfo>();
}
