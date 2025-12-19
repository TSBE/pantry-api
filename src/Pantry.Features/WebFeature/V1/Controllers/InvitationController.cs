using System;
using System.Collections.Generic;
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

[Route("api/v{version:apiVersion}/invitations")]
[ApiController]
public class InvitationController : ControllerBase
{
    private readonly ICommandPublisher _commandPublisher;

    private readonly IQueryPublisher _queryPublisher;

    public InvitationController(IQueryPublisher queryPublisher, ICommandPublisher commandPublisher)
    {
        _queryPublisher = queryPublisher;
        _commandPublisher = commandPublisher;
    }

    /// <summary>
    /// Gets the invitations for the logged in user.
    /// </summary>
    /// <returns>returns invitation.</returns>
    [HttpGet("my")]
    public async Task<Results<Ok<InvitationListResponse>, BadRequest>> GetInvitationAsync()
    {
        IEnumerable<InvitationResponse> invitations = (await _queryPublisher.ExecuteAsync(new InvitationListQuery())).ToDtos();
        return TypedResults.Ok(new InvitationListResponse { Invitations = invitations });
    }

    /// <summary>
    /// Creates a new invitation for the logged in user and returns the newly created model.
    /// </summary>
    /// <returns>invitation.</returns>
    [HttpPost]
    public async Task<Results<Ok<InvitationResponse>, BadRequest>> CreateInvitationAsync([FromBody] InvitationRequest invitationRequest)
    {
        InvitationResponse invitation = (await _commandPublisher.ExecuteAsync(new CreateInvitationCommand(invitationRequest.FriendsCode))).ToDtoNotNull();
        return TypedResults.Ok(invitation);
    }

    /// <summary>
    /// Accept the Invitation.
    /// </summary>
    /// <returns>no content.</returns>
    [HttpPost("{friendsCode:guid}/accept")]
    public async Task<Results<NoContent, BadRequest>> AcceptInvitationAsync(Guid friendsCode)
    {
        await _commandPublisher.ExecuteAsync(new AcceptInvitationCommand(friendsCode));
        return TypedResults.NoContent();
    }

    /// <summary>
    /// Decline the Invitation.
    /// </summary>
    /// <returns>no content.</returns>
    [HttpPost("{friendsCode:guid}/decline")]
    public async Task<Results<NoContent, BadRequest>> DeclineInvitationAsync(Guid friendsCode)
    {
        await _commandPublisher.ExecuteAsync(new DeclineInvitationCommand(friendsCode));
        return TypedResults.NoContent();
    }
}
