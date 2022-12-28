using System.Collections.Generic;

namespace Pantry.Features.WebFeature.V1.Controllers.Responses;

public class InvitationListResponse
{
    public IEnumerable<InvitationResponse>? Invitations { get; set; }
}
