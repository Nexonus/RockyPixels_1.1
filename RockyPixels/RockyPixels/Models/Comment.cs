using System;
using System.Collections.Generic;

namespace RockyPixels.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public string CommentContent { get; set; } = null!;

    public string? Author { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? ParentPostId { get; set; }

    public virtual Post? ParentPost { get; set; }
}
