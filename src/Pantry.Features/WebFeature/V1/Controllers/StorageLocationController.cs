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

[Route("api/v{version:apiVersion}/storage-locations")]
[ApiController]
public class StorageLocationController : ControllerBase
{
    private readonly ICommandPublisher _commandPublisher;

    private readonly IQueryPublisher _queryPublisher;

    public StorageLocationController(IQueryPublisher queryPublisher, ICommandPublisher commandPublisher)
    {
        _queryPublisher = queryPublisher;
        _commandPublisher = commandPublisher;
    }

    /// <summary>
    /// Get storage location.
    /// </summary>
    /// <returns>returns logged in users storage location.</returns>
    [HttpGet("{storageLocationId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StorageLocationResponse))]
    public async Task<IActionResult> GetStorageLocationByIdAsync(long storageLocationId)
    {
        StorageLocationResponse storageLocation = (await _queryPublisher.ExecuteAsync(new StorageLocationByIdQuery(storageLocationId))).ToDtoNotNull();
        return Ok(storageLocation);
    }

    /// <summary>
    /// Gets all storageLocations.
    /// </summary>
    /// <returns>List of all storageLocations.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StorageLocationListResponse))]
    public async Task<IActionResult> GetAllStorageLocationsAsync()
    {
        IEnumerable<StorageLocationResponse> storageLocations = (await _queryPublisher.ExecuteAsync(new StorageLocationListQuery())).ToDtos();
        return Ok(new StorageLocationListResponse { StorageLocations = storageLocations });
    }

    /// <summary>
    /// Creates storage location.
    /// </summary>
    /// <returns>storage location.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StorageLocationResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateStorageLocationAsync([FromBody] StorageLocationRequest storageLocationRequest)
    {
        StorageLocationResponse storageLocation = (await _commandPublisher.ExecuteAsync(new CreateStorageLocationCommand(storageLocationRequest.Name, storageLocationRequest.Description))).ToDtoNotNull();
        return Ok(storageLocation);
    }

    /// <summary>
    /// Update storage location.
    /// </summary>
    /// <returns>storage location.</returns>
    [HttpPut("{storageLocationId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StorageLocationResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateStorageLocationAsync([FromBody] StorageLocationRequest storageLocationRequest, long storageLocationId)
    {
        StorageLocationResponse storageLocation = (await _commandPublisher.ExecuteAsync(new UpdateStorageLocationCommand(storageLocationId, storageLocationRequest.Name, storageLocationRequest.Description))).ToDtoNotNull();
        return Ok(storageLocation);
    }

    /// <summary>
    ///  Deletes storage location.
    /// </summary>
    /// <returns>no content.</returns>
    [HttpDelete("{storageLocationId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteStorageLocationAsync(long storageLocationId)
    {
        await _commandPublisher.ExecuteAsync(new DeleteStorageLocationCommand(storageLocationId));
        return NoContent();
    }
}
