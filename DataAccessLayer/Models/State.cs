using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class State
{
    public long StateId { get; set; }

    public string? StateName { get; set; }

    public long? CountryId { get; set; }

    public virtual ICollection<BusinessInfo> BusinessInfos { get; set; } = new List<BusinessInfo>();

    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
