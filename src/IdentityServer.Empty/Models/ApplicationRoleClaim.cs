using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Empty.Models
{
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public virtual ApplicationRole? Role { get; set; }
    }
}
