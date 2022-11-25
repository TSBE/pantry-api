using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Pantry.Common;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence.Entities;
using Xunit.Abstractions;

namespace Pantry.Tests.Component.Integration.Controllers;

public abstract class BaseControllerFixture
{
    protected const string PrincipalJohnDoeId = "auth0|backdoor1234567890";
    protected const string PrincipalFooBarId = "auth0|0987654321";

    protected BaseControllerFixture(ITestOutputHelper testOutputHelper)
    {
        JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        TestOutputHelper = testOutputHelper;
        PrincipalOfJohnDoe = CreatePrincipal(PrincipalJohnDoeId);
        PrincipalOfJohnDoeWithHousehold = CreatePrincipal(PrincipalJohnDoeId, new Claim(CustomClaimTypes.HOUSEHOLDID, "1"));
    }

    protected JsonSerializerOptions JsonSerializerOptions { get; }

    protected ITestOutputHelper TestOutputHelper { get; }

    protected IPrincipal PrincipalEmpty { get; } = new ClaimsPrincipal(new ClaimsIdentity());

    protected IPrincipal PrincipalOfJohnDoeWithHousehold { get; }

    protected IPrincipal PrincipalOfJohnDoe { get; }

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

    protected StorageLocation StorageLocationOfJohnDoe { get; } = new()
    {
        StorageLocationId = 1,
        HouseholdId = 1,
        Name = "Unittest Location",
        Description = "Test Description"
    };

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
