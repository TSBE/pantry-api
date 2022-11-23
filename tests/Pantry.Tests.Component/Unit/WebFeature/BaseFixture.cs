using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;

namespace Pantry.Tests.Component.Unit.WebFeature
{
    public abstract class BaseFixture
    {
        protected const string PrincipalAuth0Id = "auth0|1234567890";

        protected BaseFixture()
        {
            PrincipalUser = CreatePrincipal(PrincipalAuth0Id);
        }

        protected IPrincipal PrincipalEmpty { get; } = new ClaimsPrincipal(new ClaimsIdentity());

        protected IPrincipal PrincipalUser { get; }

        private static IPrincipal CreatePrincipal(string userId)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)),
            };

            return new ClaimsPrincipal(new ClaimsIdentity(claims));
        }
    }
}
