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

[Route("api/v{version:apiVersion}/accounts/me")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ICommandPublisher _commandPublisher;

    private readonly IQueryPublisher _queryPublisher;

    public AccountController(IQueryPublisher queryPublisher, ICommandPublisher commandPublisher)
    {
        _queryPublisher = queryPublisher;
        _commandPublisher = commandPublisher;
    }

    /// <summary>
    /// Get my account.
    /// </summary>
    /// <returns>returns logged in users account.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountResponse))]
    public async Task<IActionResult> GetAccountAsync()
    {
        AccountResponse account = (await _queryPublisher.ExecuteAsync(new AccountQuery())).ToDtoNotNull();
        return Ok(account);
    }

    /// <summary>
    /// Creates the accoount from the logged in user if not yet done so and returns the account.
    /// </summary>
    /// <returns>account.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateAccountAsync([FromBody] AccountRequest accountRequest)
    {
        AccountResponse account = (await _commandPublisher.ExecuteAsync(new CreateAccountCommand(accountRequest.FirstName, accountRequest.LastName))).ToDtoNotNull();
        return Ok(account);
    }

    /// <summary>
    ///  Deletes the account for the logged in user.
    /// </summary>
    /// <returns>no content.</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAccountAsync()
    {
        await _commandPublisher.ExecuteAsync(new DeleteAccountCommand());
        return NoContent();
    }
}
