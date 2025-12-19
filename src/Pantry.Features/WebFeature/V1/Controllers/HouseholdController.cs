using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Pantry.Features.WebFeature.Commands;
using Pantry.Features.WebFeature.Queries;
using Pantry.Features.WebFeature.V1.Controllers.Extensions;
using Pantry.Features.WebFeature.V1.Controllers.Requests;
using Pantry.Features.WebFeature.V1.Controllers.Responses;
using Silverback.Messaging.Publishing;

namespace Pantry.Features.WebFeature.V1.Controllers;

[Route("api/v{version:apiVersion}/households/my")]
[ApiController]
public class HouseholdController : ControllerBase
{
    private readonly ICommandPublisher _commandPublisher;

    private readonly IQueryPublisher _queryPublisher;

    public HouseholdController(IQueryPublisher queryPublisher, ICommandPublisher commandPublisher)
    {
        _queryPublisher = queryPublisher;
        _commandPublisher = commandPublisher;
    }

    /// <summary>
    /// Get my household.
    /// </summary>
    /// <returns>returns logged in users household.</returns>
    [HttpGet]
    public async Task<Results<Ok<HouseholdResponse>, BadRequest>> GetHouseholdAsync()
    {
        HouseholdResponse household = (await _queryPublisher.ExecuteAsync(new HouseholdQuery())).ToDtoNotNull();
        return TypedResults.Ok(household);
    }

    /// <summary>
    /// Creates household from the logged in user if not yet done so and returns the household.
    /// </summary>
    /// <returns>household.</returns>
    [HttpPost]
    public async Task<Results<Ok<HouseholdResponse>, BadRequest>> CreateHouseholdAsync([FromBody] HouseholdRequest householdRequest)
    {
        HouseholdResponse household = (await _commandPublisher.ExecuteAsync(new CreateHouseholdCommand(householdRequest.Name, householdRequest.SubscriptionType.ToModelNotNull()))).ToDtoNotNull();
        return TypedResults.Ok(household);
    }
}
