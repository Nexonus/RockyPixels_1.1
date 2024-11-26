using System;
using System.Collections.Generic;

namespace RockyPixels.Models;

public partial class UserDatum
{
    public int UserId { get; set; }

    public string IconPath { get; set; } = null!;

    public string IconFilename { get; set; } = null!;

    public string? Description { get; set; }

    public virtual User? User { get; set; }
}
