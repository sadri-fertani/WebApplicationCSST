using WebApplicationCSST.API.Provider.Role;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RoleAuthorizationServiceCollectionExtensions
    {
        /// <summary>
        /// Activates role authorization for Windows authentication for the ASP.Net Core web site.
        /// </summary>
        /// <typeparam name="TRoleProvider">The <see cref="Type"/> of the <see cref="IRoleProvider"/> implementation that will provide user roles.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> onto which to register the services.</param>
        public static IServiceCollection AddRoleAuthorization<TRoleProvider>(this IServiceCollection services)
            where TRoleProvider : class, IRoleProvider
        {
            services
                .AddSingleton<IRoleProvider, TRoleProvider>();

            services
                .AddSingleton<IClaimsTransformation, RoleAuthorizationTransform>();

            return services;
        }
    }
}
