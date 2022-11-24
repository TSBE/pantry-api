using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pantry.Features.WebFeature.Commands;
using Pantry.Features.WebFeature.Queries;
using Pantry.Features.WebFeature.V1.Controllers.Mappers;
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HouseholdResponse))]
    public async Task<IActionResult> GetHouseholdAsync()
    {
        HouseholdResponse household = (await _queryPublisher.ExecuteAsync(new HouseholdQuery())).ToDtoNotNull();
        return Ok(household);
    }

    /// <summary>
    /// Creates household from the logged in user if not yet done so and returns the household.
    /// </summary>
    /// <returns>household.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HouseholdResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateHouseholdAsync([FromBody] HouseholdRequest householdRequest)
    {
        HouseholdResponse household = (await _commandPublisher.ExecuteAsync(new CreateHouseholdCommand(householdRequest.Name, householdRequest.SubscriptionType.ToModelNotNull()))).ToDtoNotNull();
        return Ok(household);
    }
}
