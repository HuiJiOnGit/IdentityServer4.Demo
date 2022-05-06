using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Empty.Models
{
    public class ApplicationUserToken : IdentityUserToken<string>
    {
        public virtual ApplicationUser? User { get; set; }
    }
}
