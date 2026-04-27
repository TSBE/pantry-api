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

[Route("api/v{version:apiVersion}/devices")]
[ApiController]
public class DeviceController : ControllerBase
{
    private readonly IPublisher _publisher;

    public DeviceController(IPublisher publisher)
    {
        _publisher = publisher;
    }

    /// <summary>
    /// Gets all devices.
    /// </summary>
    /// <returns>List of all devices.</returns>
    [HttpGet]
    public async Task<Results<Ok<DeviceListResponse>, BadRequest>> GetAllDevicesAsync()
    {
        IEnumerable<DeviceResponse> devices = (await _publisher.ExecuteQueryAsync(new DeviceListQuery())).ToDtos();
        return TypedResults.Ok(new DeviceListResponse { Devices = devices });
    }

    /// <summary>
    /// Gets device by installationId.
    /// </summary>
    /// <returns>List of all devices.</returns>
    [HttpGet("{installationId:guid}")]
    public async Task<Results<Ok<DeviceResponse>, BadRequest>> GetDeviceByIdAsync(Guid installationId)
    {
        DeviceResponse device = (await _publisher.ExecuteQueryAsync(new DeviceByIdQuery(installationId))).ToDtoNotNull();
        return TypedResults.Ok(device);
    }

    /// <summary>
    /// Create device.
    /// </summary>
    /// <returns>device.</returns>
    [HttpPost]
    public async Task<Results<Ok<DeviceResponse>, BadRequest>> CreateDeviceAsync([FromBody] DeviceRequest deviceRequest)
    {
        DeviceResponse device = (await _publisher.ExecuteCommandAsync(
            new CreateDeviceCommand(
                deviceRequest.InstallationId,
                deviceRequest.Model,
                deviceRequest.Name,
                deviceRequest.Platform.ToModelNotNull(),
                deviceRequest.DeviceToken))).ToDtoNotNull();

        return TypedResults.Ok(device);
    }

    /// <summary>
    /// Update device.
    /// </summary>
    /// <returns>device.</returns>
    [HttpPut("{installationId:guid}")]
    public async Task<Results<Ok<DeviceResponse>, BadRequest>> UpdateDeviceAsync([FromBody] DeviceUpdateRequest deviceUpdateRequest, Guid installationId)
    {
        DeviceResponse device = (await _publisher.ExecuteCommandAsync(new UpdateDeviceCommand(installationId, deviceUpdateRequest.Name, deviceUpdateRequest.DeviceToken))).ToDtoNotNull();
        return TypedResults.Ok(device);
    }

    /// <summary>
    /// Delete device.
    /// </summary>
    /// <returns>no content.</returns>
    [HttpDelete("{installationId:guid}")]
    public async Task<Results<NoContent, BadRequest>> DeleteDeviceAsync(Guid installationId)
    {
        await _publisher.ExecuteCommandAsync(new DeleteDeviceCommand(installationId));
        return TypedResults.NoContent();
    }
}
