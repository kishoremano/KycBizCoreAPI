﻿using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Log
{
    public int Id { get; set; }

    public string Message { get; set; } = null!;

    public string? MessageTemplate { get; set; }

    public string? Level { get; set; }

    public DateTimeOffset TimeStamp { get; set; }

    public string? Exception { get; set; }

    public string? Properties { get; set; }

    public string? LogEvent { get; set; }
}
