using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IDM.WebApi.Claim
{
    public class UserClaimProvider : IClaimsTransformation
    {
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsIdentity identity = principal.Identity as ClaimsIdentity;
            System.Security.Claims.Claim claim = null;
            if (principal.HasClaim(x => x.Type == "username"))
            {
                claim = new System.Security.Claims.Claim("username", identity.Name);
                identity.AddClaim(claim);
            }
            if (principal.HasClaim(x => x.Type == "logintime"))
            {
                claim = new System.Security.Claims.Claim("logintime", DateTime.Now.ToString());
                identity.AddClaim(claim);
            }

            return principal;
        }
    }
}
