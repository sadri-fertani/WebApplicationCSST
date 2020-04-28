using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace WebApplicationCSST.API.Provider
{
    /// <summary>
    /// Implements a <see cref="IClaimsTransformation" /> that uses a <see cref="IRoleProvider" /> to fetch and apply
    /// applicative roles for a user.
    /// <para>
    /// To use, you need to implement a class that inherit from <see cref="IRoleProvider" /> and use the
    /// <see cref="RoleAuthorizationService.AddRoleAuthorization{TRoleProvider}" /> extension method
    /// in the <c>ConfigureServices</c> method of the <c>Startup</c> class to enable the role authorization and
    /// associate your role provider implementation.
    /// </para>
    /// </summary>
    public class RoleAuthorizationTransform : IClaimsTransformation
    {
        private static readonly string RoleClaimType = $"http://{typeof(RoleAuthorizationTransform).FullName.Replace('.', '/')}/role";
        private readonly IRoleProvider _roleProvider;

        public RoleAuthorizationTransform(IRoleProvider roleProvider)
        {
            _roleProvider = roleProvider ?? throw new ArgumentNullException(nameof(roleProvider));
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            // Cast the principal identity to a Claims identity to access claims etc...
            var oldIdentity = (ClaimsIdentity)principal.Identity;

            // "Clone" the old identity to avoid nasty side effects.
            // NB: We take a chance to replace the claim type used to define the roles with our own.
            var newIdentity = new ClaimsIdentity(
                oldIdentity.Claims,
                oldIdentity.AuthenticationType,
                oldIdentity.NameClaimType,
                RoleClaimType);

            // Fetch the roles for the user and add the claims of the correct type so that roles can be recognized.
            var roles = await _roleProvider.GetUserRolesAsync(newIdentity);
            newIdentity.AddClaims(roles.Select(r => new Claim(RoleClaimType, r)));

            // Create and return a new claims principal
            return new ClaimsPrincipal(newIdentity);
        }
    }
}