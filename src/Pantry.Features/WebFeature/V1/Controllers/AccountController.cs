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
    private readonly IPublisher _publisher;

    public AccountController(IPublisher publisher)
    {
        _publisher = publisher;
    }

    /// <summary>
    /// Get my account.
    /// </summary>
    /// <returns>returns logged in users account.</returns>
    [HttpGet]
    public async Task<Results<Ok<AccountResponse>, NotFound>> GetAccountAsync()
    {
        AccountResponse account = (await _publisher.ExecuteQueryAsync(new AccountQuery())).ToDtoNotNull();
        return TypedResults.Ok(account);
    }

    /// <summary>
    /// Creates the accoount from the logged in user if not yet done so and returns the account.
    /// </summary>
    /// <returns>account.</returns>
    [HttpPut]
    public async Task<Results<Ok<AccountResponse>, BadRequest>> CreateAccountAsync([FromBody] AccountRequest accountRequest)
    {
        AccountResponse account = (await _publisher.ExecuteCommandAsync(new CreateAccountCommand(accountRequest.FirstName, accountRequest.LastName))).ToDtoNotNull();
        return TypedResults.Ok(account);
    }

    /// <summary>
    ///  Deletes the account for the logged in user.
    /// </summary>
    /// <returns>no content.</returns>
    [HttpDelete]
    public async Task<Results<NoContent, BadRequest>> DeleteAccountAsync()
    {
        await _publisher.ExecuteCommandAsync(new DeleteAccountCommand());
        return TypedResults.NoContent();
    }
}
