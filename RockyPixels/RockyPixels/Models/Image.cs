using System;
using System.Collections.Generic;

namespace RockyPixels.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public byte[] ImageData { get; set; } = null!;

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
