using System;
using System.Collections.Generic;
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InvitationResponse))]
    public async Task<IActionResult> GetInvitationAsync()
    {
        IEnumerable<InvitationResponse> invitations = (await _queryPublisher.ExecuteAsync(new InvitationListQuery())).ToDtos();
        return Ok(new InvitationListResponse { Invitations = invitations });
    }

    /// <summary>
    /// Creates a new invitation for the logged in user and returns the newly created model.
    /// </summary>
    /// <returns>invitation.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InvitationResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateInvitationAsync([FromBody] InvitationRequest invitationRequest)
    {
        InvitationResponse invitation = (await _commandPublisher.ExecuteAsync(new CreateInvitationCommand(invitationRequest.FriendsCode))).ToDtoNotNull();
        return Ok(invitation);
    }

    /// <summary>
    /// Accept the Invitation.
    /// </summary>
    /// <returns>no content.</returns>
    [HttpPost("{friendsCode}/accept")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> AcceptInvitationAsync(Guid friendsCode)
    {
        await _commandPublisher.ExecuteAsync(new AcceptInvitationCommand(friendsCode));
        return NoContent();
    }

    /// <summary>
    /// Decline the Invitation.
    /// </summary>
    /// <returns>no content.</returns>
    [HttpPost("{friendsCode}/decline")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DeclineInvitationAsync(Guid friendsCode)
    {
        await _commandPublisher.ExecuteAsync(new DeclineInvitationCommand(friendsCode));
        return NoContent();
    }
}
