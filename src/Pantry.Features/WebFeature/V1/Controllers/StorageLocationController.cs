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
    [HttpGet("{storageLocationId:long}")]
    public async Task<Results<Ok<StorageLocationResponse>, BadRequest>> GetStorageLocationByIdAsync(long storageLocationId)
    {
        StorageLocationResponse storageLocation = (await _queryPublisher.ExecuteAsync(new StorageLocationByIdQuery(storageLocationId))).ToDtoNotNull();
        return TypedResults.Ok(storageLocation);
    }

    /// <summary>
    /// Gets all storageLocations.
    /// </summary>
    /// <returns>List of all storageLocations.</returns>
    [HttpGet]
    public async Task<Results<Ok<StorageLocationListResponse>, BadRequest>> GetAllStorageLocationsAsync()
    {
        IEnumerable<StorageLocationResponse> storageLocations = (await _queryPublisher.ExecuteAsync(new StorageLocationListQuery())).ToDtos();
        return TypedResults.Ok(new StorageLocationListResponse { StorageLocations = storageLocations });
    }

    /// <summary>
    /// Creates storage location.
    /// </summary>
    /// <returns>storage location.</returns>
    [HttpPost]
    public async Task<Results<Ok<StorageLocationResponse>, BadRequest>> CreateStorageLocationAsync([FromBody] StorageLocationRequest storageLocationRequest)
    {
        StorageLocationResponse storageLocation = (await _commandPublisher.ExecuteAsync(new CreateStorageLocationCommand(storageLocationRequest.Name, storageLocationRequest.Description))).ToDtoNotNull();
        return TypedResults.Ok(storageLocation);
    }

    /// <summary>
    /// Update storage location.
    /// </summary>
    /// <returns>storage location.</returns>
    [HttpPut("{storageLocationId:long}")]
    public async Task<Results<Ok<StorageLocationResponse>, BadRequest>> UpdateStorageLocationAsync([FromBody] StorageLocationRequest storageLocationRequest, long storageLocationId)
    {
        StorageLocationResponse storageLocation = (await _commandPublisher.ExecuteAsync(new UpdateStorageLocationCommand(storageLocationId, storageLocationRequest.Name, storageLocationRequest.Description))).ToDtoNotNull();
        return TypedResults.Ok(storageLocation);
    }

    /// <summary>
    ///  Deletes storage location.
    /// </summary>
    /// <returns>no content.</returns>
    [HttpDelete("{storageLocationId:long}")]
    public async Task<Results<NoContent, BadRequest>> DeleteStorageLocationAsync(long storageLocationId)
    {
        await _commandPublisher.ExecuteAsync(new DeleteStorageLocationCommand(storageLocationId));
        return TypedResults.NoContent();
    }
}
