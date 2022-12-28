using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pantry.Features.WebFeature.Queries;
using Pantry.Features.WebFeature.V1.Controllers.Mappers;
using Pantry.Features.WebFeature.V1.Controllers.Responses;
using Silverback.Messaging.Publishing;

namespace Pantry.Features.WebFeature.V1.Controllers;

[Route("api/v{version:apiVersion}/metadatas")]
[ApiController]
public class MetadataController : ControllerBase
{
    private readonly IQueryPublisher _queryPublisher;

    public MetadataController(IQueryPublisher queryPublisher)
    {
        _queryPublisher = queryPublisher;
    }

    /// <summary>
    /// Get metadata.
    /// </summary>
    /// <returns>returns article metadata.</returns>
    [HttpGet("{barcode}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MetadataResponse))]
    public async Task<IActionResult> GetMetadataByGtinAsync(string barcode)
    {
        MetadataResponse metadata = (await _queryPublisher.ExecuteAsync(new MetadataByGtinQuery(barcode))).ToDtoNotNull();
        return Ok(metadata);
    }
}
