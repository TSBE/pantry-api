using System.Collections.Generic;

namespace Pantry.Features.WebFeature.V1.Controllers.Responses;

public class StorageLocationListResponse
{
    public IEnumerable<StorageLocationResponse>? StorageLocations { get; set; }
}
