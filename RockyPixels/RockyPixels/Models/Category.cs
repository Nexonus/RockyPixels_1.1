using System;
using System.Collections.Generic;

namespace RockyPixels.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
