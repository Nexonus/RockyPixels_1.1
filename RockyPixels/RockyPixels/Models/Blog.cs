using System;
using System.Collections.Generic;

namespace RockyPixels.Models;

public partial class Blog
{
    public int? PostId { get; set; }

    public int? TagId { get; set; }

    public virtual Post? Post { get; set; }

    public virtual Tag? Tag { get; set; }
}
