using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RockyPixels.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public byte[]? Data { get; set; }

    [NotMapped]
    public IFormFile? ImageForm { get; set; }
    public string? Metadata { get; set; }

    public int? PostId { get; set; }
}
