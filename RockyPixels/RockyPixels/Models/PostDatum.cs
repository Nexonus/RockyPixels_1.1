using System;
using System.Collections.Generic;

namespace RockyPixels.Models;

public partial class PostDatum
{
    public int AttachmentId { get; set; }

    public string AttachmentPath { get; set; } = null!;

    public string AttachmentFilename { get; set; } = null!;

    public string? AttachmentMetadata { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
