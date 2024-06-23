using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class City
{
    public long CityId { get; set; }

    public string? CityName { get; set; }

    public long? StateId { get; set; }

    public virtual ICollection<BusinessInfo> BusinessInfos { get; set; } = new List<BusinessInfo>();

    public virtual State? State { get; set; }
}
