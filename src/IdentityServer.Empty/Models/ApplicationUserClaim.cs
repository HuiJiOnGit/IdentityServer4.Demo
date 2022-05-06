using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Empty.Models
{
    public class ApplicationUserClaim : IdentityUserClaim<string>
    {
        public virtual ApplicationUser? User { get; set; }
    }
}
