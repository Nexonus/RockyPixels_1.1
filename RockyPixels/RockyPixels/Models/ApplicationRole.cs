using Microsoft.AspNetCore.Identity;

namespace RockyPixels.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string Id { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
