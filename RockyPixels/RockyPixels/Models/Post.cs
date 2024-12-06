using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RockyPixels.Models;

public partial class Post
{
    [Display(Name = "Post ID")]
    public int PostId { get; set; }

    [Display(Name = "Title")]
    public string Topic { get; set; } = null!;

    [Display(Name = "Post")]
    public string PostContent { get; set; } = null!;

    [Display(Name = "Creation Date")]
    public DateTime CreatedOn { get; set; }

    [Display(Name = "Modification Date")]
    public DateTime? LastModifiedOn { get; set; }

    [Display(Name = "Post Author")]
    public string? Author { get; set; }

    [Display(Name="Image")]
    public byte[]? ImageData { get; set; }

    public int? CategoryId { get; set; }

    [Display(Name = "Image ID")]
    public int? ImageId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Image? Image { get; set; }

    [Display(Name = "Tags")]
    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();

    [NotMapped]
    public IFormFile? ImageForm { get; set; }
}
