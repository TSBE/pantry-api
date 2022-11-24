using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using Pantry.Core.Persistence.Entities;

namespace Pantry.Tests.Component.Unit.WebFeature
{
    public abstract class BaseFixture
    {
        protected const string PrincipalJohnDoeId = "auth0|1234567890";
        protected const string PrincipalTestUser1Id = "auth0|1234567890testuser1";

        protected BaseFixture()
        {
            PrincipalOfJohnDoe = CreatePrincipal(PrincipalJohnDoeId);
            PrincipalTestUser1 = CreatePrincipal(PrincipalTestUser1Id);
        }

        protected Account AccountJohnDoe { get; } = new()
        {
            AccountId = 1,
            FirstName = "John",
            LastName = "Doe",
            FriendsCode = Guid.NewGuid(),
            OAuhtId = PrincipalJohnDoeId
        };

        protected IPrincipal PrincipalEmpty { get; } = new ClaimsPrincipal(new ClaimsIdentity());

        protected IPrincipal PrincipalOfJohnDoe { get; }

        protected IPrincipal PrincipalTestUser1 { get; }

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
