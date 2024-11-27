using System;
using System.Collections.Generic;

namespace RockyPixels.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public byte[]? Avatar { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual Role Role { get; set; } = null!;
}
