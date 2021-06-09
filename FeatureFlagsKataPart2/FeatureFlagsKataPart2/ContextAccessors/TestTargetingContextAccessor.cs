using FeatureFlagsKataPart2.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement.FeatureFilters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeatureFlagsKataPart2.ContextAccessors
{
    public class TestTargetingContextAccessor : ITargetingContextAccessor
    {
        private const string TargetingContextLookup = "TestTargetingContextAccessor.TargetingContext";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public TestTargetingContextAccessor(
            IHttpContextAccessor httpContextAccessor
            , IServiceProvider serviceProvider
            )
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serviceProvider = serviceProvider;
        }

        public async ValueTask<TargetingContext> GetContextAsync()
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext;
            if (httpContext.Items.TryGetValue(TargetingContextLookup, out object value))
            {
                return (TargetingContext)value;
            }
            List<string> groups = new List<string>();
            if (httpContext.User.Identity.Name != null)
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                groups.Add((await userManager.FindByNameAsync(httpContext.User.Identity.Name)).TenantId.ToString());
            }
            TargetingContext targetingContext = new TargetingContext
            {
                UserId = httpContext.User.Identity.Name,
                Groups = groups
            };
            httpContext.Items[TargetingContextLookup] = targetingContext;
            return targetingContext;
        }
    }
}
