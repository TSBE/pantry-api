using System.Collections.Generic;

namespace Pantry.Features.WebFeature.V1.Controllers.Responses;

public class DeviceListResponse
{
    public IEnumerable<DeviceResponse>? Devices { get; set; }
}
