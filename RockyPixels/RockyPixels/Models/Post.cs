using System;
using System.Collections.Generic;

namespace RockyPixels.Models;

public partial class Post
{
    public int PostId { get; set; }

    public string? Topic { get; set; }

    public string? PostContent { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public string? ImageId { get; set; }

    public string? UserId { get; set; }

    public virtual AspNetUser? User { get; set; }
}
