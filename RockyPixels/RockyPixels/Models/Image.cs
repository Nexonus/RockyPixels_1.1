using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RockyPixels.Models;

public partial class Image
{
    [Display(Name = "Image ID")]
    public int ImageId { get; set; }

    [Display(Name = "Image")]
    public byte[]? ImageData { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
