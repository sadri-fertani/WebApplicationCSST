using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApplicationCSST.API.Provider.Role
{
    public interface IRoleProvider
    {
        Task<IList<string>> GetUserRolesAsync(ClaimsIdentity identity);
    }
}
