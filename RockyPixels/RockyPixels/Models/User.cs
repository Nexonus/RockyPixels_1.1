using System;
using System.Collections.Generic;

namespace RockyPixels.Models;

public partial class User
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string? IconPath { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
