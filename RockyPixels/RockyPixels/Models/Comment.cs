using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RockyPixels.Models;

public partial class Comment
{
    [Display(Name = "Comment ID")]
    public int CommentId { get; set; }

    [Display(Name = "Comment")]
    public string CommentContent { get; set; } = null!;

    [Display(Name = "Comment Author")]
    public string? Author { get; set; }

    [Display(Name = "Commented On")]
    public DateTime CreatedOn { get; set; }

    public int? ParentPostId { get; set; }

    [Display(Name = "Post ID")]
    public virtual Post? ParentPost { get; set; }
}
