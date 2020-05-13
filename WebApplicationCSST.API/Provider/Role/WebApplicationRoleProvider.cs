using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace WebApplicationCSST.API.Provider.Role
{
    public class WebApplicationRoleProvider : IRoleProvider
    {
        public const string SUPER_ADMIN = "SuperAdmin";
        public const string ADMIN = "Admin";
        public const string BASIC_USER = "BasicUser";

        public Task<IList<string>> GetUserRolesAsync(ClaimsIdentity identity)
        {
            IList<string> result = new List<string>();

            if (identity.IsAuthenticated)
            {
                // Add default use role (because authenticated)
                result.Add(BASIC_USER);

                // ...
                string userEmail = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (userEmail.ToLowerInvariant().Equals("cedric_sf@hotmail.com"))
                    result.Add(SUPER_ADMIN);
            }

            return Task.FromResult(result);
        }
    }
}
