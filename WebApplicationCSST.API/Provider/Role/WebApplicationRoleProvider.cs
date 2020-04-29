using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace WebApplicationCSST.API.Provider.Role
{
    public class WebApplicationRoleProvider : IRoleProvider
    {
        public const string ADMIN = "Admin";
        public const string BASIC_USER = "BasicUser";

        public Task<IList<string>> GetUserRolesAsync(ClaimsIdentity identity)
        {
            IList<string> result = new List<string>();

            // Add default use role (because authenticated)
            result.Add(BASIC_USER);

            // Get groups name
            var groups = GetGroupName(WindowsIdentity.GetCurrent().Groups);

            if (groups.Contains(@"INTRA\G_VPN_Pandemie") || groups.Contains(@"MicrosoftAccount\sadri.fertani@live.fr"))
                result.Add(ADMIN);

            return Task.FromResult(result);
        }

        private IList<string> GetGroupName(IdentityReferenceCollection identityReferences)
        {
            IList<string> result = new List<string>();

            if (identityReferences != null)
            {
                foreach (var group in identityReferences)
                {
                    try
                    {
                        result.Add(group.Translate(typeof(NTAccount)).ToString());
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            return result;
        }
    }
}
