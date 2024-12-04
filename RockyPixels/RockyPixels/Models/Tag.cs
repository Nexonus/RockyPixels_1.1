using System;
using System.Collections.Generic;

namespace RockyPixels.Models;

public partial class Tag
{
    public int TagId { get; set; }

    public string? TagName { get; set; }

    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}
