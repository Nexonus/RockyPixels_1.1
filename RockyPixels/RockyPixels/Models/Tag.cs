using System;
using System.Collections.Generic;

namespace RockyPixels.Models;

public partial class Tag
{
    public int TagId { get; set; }

    public string? TagValue { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
