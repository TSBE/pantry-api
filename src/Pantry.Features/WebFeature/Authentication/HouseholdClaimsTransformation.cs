using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;

namespace Pantry.Features.WebFeature.Authentication;

public class HouseholdClaimsTransformation : IClaimsTransformation
{
    private readonly ILogger<HouseholdClaimsTransformation> _logger;

    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public HouseholdClaimsTransformation(
        ILogger<HouseholdClaimsTransformation> logger,
        IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var claimsIdentity = principal.Identity as ClaimsIdentity;
        var authId = (principal as IPrincipal).GetAuth0Id();

        if (claimsIdentity is not null && authId is not null)
        {
            using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();
            var householdId = appDbContext.Accounts.FirstOrDefault(x => x.OAuhtId == authId)?.HouseholdId ?? default;

            if (householdId > 0)
            {
                claimsIdentity.AddClaim(new Claim(CustomClaimTypes.HOUSEHOLDID, householdId.ToString(CultureInfo.InvariantCulture)));
                principal.AddIdentity(claimsIdentity);
            }
        }

        return Task.FromResult(principal);
    }
}
