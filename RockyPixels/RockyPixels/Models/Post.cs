using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RockyPixels.Models;

public partial class Post
{
    public int Id { get; set; }

    public string? Topic { get; set; }

    [DisplayName("Content")]
    public string? PostContent { get; set; }

    [DisplayName("Creation Date")]
    public DateTime CreatedOn { get; set; }

    [DisplayName("Modification Date")]
    public DateTime? LastModifiedOn { get; set; }

    public int UserId { get; set; }

    public virtual User? User { get; set; }
}
