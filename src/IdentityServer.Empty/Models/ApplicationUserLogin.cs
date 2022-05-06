using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Empty.Models
{
    public class ApplicationUserLogin : IdentityUserLogin<string>
    {
        public virtual ApplicationUser? User { get; set; }
    }
}