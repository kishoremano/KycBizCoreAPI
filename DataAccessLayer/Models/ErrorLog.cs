using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class ErrorLog
{
    public decimal ErrorId { get; set; }

    public int? ErrorType { get; set; }

    public decimal? ModuleId { get; set; }

    public int? UserType { get; set; }

    public decimal? UserId { get; set; }

    public string? ErrorMsg { get; set; }

    public DateTime? ErrorDate { get; set; }

    public string? Ipaddress { get; set; }

    public string? PageName { get; set; }

    public string? FunctionName { get; set; }

    public int? ErrorStatus { get; set; }

    public int? Status { get; set; }

    public DateTime? ModifiedOn { get; set; }
}
