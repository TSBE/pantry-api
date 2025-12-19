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
    public async Task<Results<Ok<AccountResponse>, NotFound>> GetAccountAsync()
    {
        AccountResponse account = (await _queryPublisher.ExecuteAsync(new AccountQuery())).ToDtoNotNull();
        return TypedResults.Ok(account);
    }

    /// <summary>
    /// Creates the accoount from the logged in user if not yet done so and returns the account.
    /// </summary>
    /// <returns>account.</returns>
    [HttpPut]
    public async Task<Results<Ok<AccountResponse>, BadRequest>> CreateAccountAsync([FromBody] AccountRequest accountRequest)
    {
        AccountResponse account = (await _commandPublisher.ExecuteAsync(new CreateAccountCommand(accountRequest.FirstName, accountRequest.LastName))).ToDtoNotNull();
        return TypedResults.Ok(account);
    }

    /// <summary>
    ///  Deletes the account for the logged in user.
    /// </summary>
    /// <returns>no content.</returns>
    [HttpDelete]
    public async Task<Results<NoContent, BadRequest>> DeleteAccountAsync()
    {
        await _commandPublisher.ExecuteAsync(new DeleteAccountCommand());
        return TypedResults.NoContent();
    }
}
