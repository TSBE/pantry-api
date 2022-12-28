using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence.Entities;

namespace Pantry.Tests.Component.Unit.WebFeature
{
    public abstract class BaseFixture
    {
        protected const string PrincipalJohnDoeId = "auth0|1234567890";
        protected const string PrincipalFooBarId = "auth0|0987654321";

        protected const string PrincipalTestUser1Id = "auth0|1234567890testuser1";

        protected BaseFixture()
        {
            PrincipalOfJohnDoe = CreatePrincipal(PrincipalJohnDoeId);
            PrincipalOfJohnDoeWithHousehold = CreatePrincipal(PrincipalJohnDoeId, new Claim(CustomClaimTypes.HOUSEHOLDID, "1"));
            PrincipalOfFooBar = CreatePrincipal(PrincipalFooBarId);
            PrincipalOfFooBarWithHousehold = CreatePrincipal(PrincipalFooBarId, new Claim(CustomClaimTypes.HOUSEHOLDID, "2"));
            PrincipalOfFooBarWithJohnDoesHousehold = CreatePrincipal(PrincipalFooBarId, new Claim(CustomClaimTypes.HOUSEHOLDID, "1"));
            PrincipalAuthenticatedUser1 = CreatePrincipal(PrincipalTestUser1Id);
        }

        protected Account AccountJohnDoe { get; } = new()
        {
            AccountId = 1,
            FirstName = "John",
            LastName = "Doe",
            FriendsCode = Guid.NewGuid(),
            OAuhtId = PrincipalJohnDoeId
        };

        protected Account AccountFooBar { get; } = new()
        {
            AccountId = 2,
            FirstName = "Foo",
            LastName = "Bar",
            FriendsCode = Guid.NewGuid(),
            OAuhtId = PrincipalFooBarId
        };

        protected Household HouseholdOfJohnDoe { get; } = new()
        {
            HouseholdId = 1,
            Name = "John's household",
            SubscriptionType = Core.Persistence.Enums.SubscriptionType.FREE,
            OwnerId = 1
        };

        protected Household HouseholdOfFooBar { get; } = new()
        {
            HouseholdId = 2,
            Name = "Foo's household",
            SubscriptionType = Core.Persistence.Enums.SubscriptionType.FREE,
            OwnerId = 2
        };

        protected StorageLocation StorageLocationOfJohnDoe { get; } = new()
        {
            StorageLocationId = 1,
            HouseholdId = 1,
            Name = "Unittest Location",
            Description = "Test Description"
        };

        protected IPrincipal PrincipalEmpty { get; } = new ClaimsPrincipal(new ClaimsIdentity());

        protected IPrincipal PrincipalOfJohnDoe { get; }

        protected IPrincipal PrincipalOfJohnDoeWithHousehold { get; }

        protected IPrincipal PrincipalOfFooBar { get; }

        protected IPrincipal PrincipalOfFooBarWithHousehold { get; }

        protected IPrincipal PrincipalOfFooBarWithJohnDoesHousehold { get; }


        protected IPrincipal PrincipalAuthenticatedUser1 { get; }

        private static IPrincipal CreatePrincipal(string userId, params Claim[] moreClaims)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)),
            };

            claims.AddRange(moreClaims);

            return new ClaimsPrincipal(new ClaimsIdentity(claims));
        }
    }
}
