using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RockyPixels.Models;

public partial class Tag
{
    [Display(Name = "Tag ID")]
    public int TagId { get; set; }

    [Display(Name = "Tag Name")]
    public string? TagName { get; set; }

    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}
