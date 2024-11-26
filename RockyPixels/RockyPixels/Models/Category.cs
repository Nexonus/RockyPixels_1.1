using System;
using System.Collections.Generic;

namespace RockyPixels.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryValue { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
