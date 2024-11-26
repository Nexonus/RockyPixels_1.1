using System;
using System.Collections.Generic;

namespace RockyPixels.Models;

public partial class Post
{
    public int PostId { get; set; }

    public int? UserId { get; set; }

    public string? Topic { get; set; }

    public string? PostContent { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? EditDate { get; set; }

    public DateTime? DeleteDate { get; set; }

    public int? CategoryId { get; set; }

    public int? TagId { get; set; }

    public int? AttachmentId { get; set; }

    public virtual PostDatum? Attachment { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Tag? Tag { get; set; }

    public virtual User? User { get; set; }
}
