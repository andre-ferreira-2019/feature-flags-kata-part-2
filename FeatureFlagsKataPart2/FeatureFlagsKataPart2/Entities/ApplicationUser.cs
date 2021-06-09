using Microsoft.AspNetCore.Identity;

namespace FeatureFlagsKataPart2.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public int TenantId { get; set; }
    }
}
