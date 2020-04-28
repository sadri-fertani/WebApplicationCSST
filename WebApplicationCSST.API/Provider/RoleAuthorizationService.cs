using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplicationCSST.API.Provider
{
    /// <summary>
    /// Provides the extension methods to enable and register the role authentication on an Asp.Net Core web site.
    /// </summary>
    public static class RoleAuthorizationService
    {
        /// <summary>
        /// Activates role authorization for Windows authentication for the ASP.Net Core web site.
        /// </summary>
        /// <typeparam name="TRoleProvider">The <see cref="Type"/> of the <see cref="IRoleProvider"/> implementation that will provide user roles.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> onto which to register the services.</param>
        public static void AddRoleAuthorization<TRoleProvider>(this IServiceCollection services)
            where TRoleProvider : class, IRoleProvider
        {
            services.AddSingleton<IRoleProvider, TRoleProvider>();
            services.AddSingleton<IClaimsTransformation, RoleAuthorizationTransform>();
        }
    }
}