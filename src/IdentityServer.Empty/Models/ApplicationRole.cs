using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Empty.Models
{
    public class ApplicationRole : IdentityRole
    {
        public virtual ICollection<ApplicationUserRole>? UserRoles { get; set; }
        public virtual ICollection<ApplicationRoleClaim>? RoleClaims { get; set; }
    }
}