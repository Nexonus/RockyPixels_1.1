using System;
using System.Collections.Generic;

namespace RockyPixels.Models;

public partial class Post
{
    public int Id { get; set; }

    public string? Topic { get; set; }

    public string? PostContent { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public int UserId { get; set; }

    public int? ImageId { get; set; }

    public virtual Image? Image { get; set; }

    public virtual User? User { get; set; }
}
