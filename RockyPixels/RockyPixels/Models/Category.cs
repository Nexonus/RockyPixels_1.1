using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RockyPixels.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    [Display(Name = "Category Name")]
    public string? CategoryName { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
