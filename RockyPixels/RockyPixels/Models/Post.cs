using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RockyPixels.Models;

public partial class Post
{
    public int PostId { get; set; }

    public string Topic { get; set; } = null!;

    public string PostContent { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public string? Author { get; set; }

    public byte[]? ImageData { get; set; }

    public int? CategoryId { get; set; }

    public int? ImageId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Image? Image { get; set; }

    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();

    [NotMapped]
    public IFormFile? ImageForm { get; set; }
}
