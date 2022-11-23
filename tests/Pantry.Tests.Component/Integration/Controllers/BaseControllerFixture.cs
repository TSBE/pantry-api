﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using Pantry.Core.Persistence.Entities;
using Xunit.Abstractions;

namespace Pantry.Tests.Component.Integration.Controllers;

public abstract class BaseControllerFixture
{
    protected const string PrincipalAuth0Id = "auth0|backdoor1234567890";

    protected BaseControllerFixture(ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;
        PrincipalUser = CreatePrincipal(PrincipalAuth0Id);
    }

    protected ITestOutputHelper TestOutputHelper { get; }

    protected IPrincipal PrincipalEmpty { get; } = new ClaimsPrincipal(new ClaimsIdentity());

    protected IPrincipal PrincipalUser { get; }

    protected Account AccountJohnDoe { get; } = new()
    {
        AccountId = 1,
        FirstName = "John",
        LastName = "Doe",
        FriendsCode = Guid.NewGuid(),
        OAuhtId = PrincipalAuth0Id
    };

    private static IPrincipal CreatePrincipal(string userId)
    {
        var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)),
            };

        return new ClaimsPrincipal(new ClaimsIdentity(claims));
    }
}
