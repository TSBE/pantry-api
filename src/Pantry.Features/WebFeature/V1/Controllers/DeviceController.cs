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

[Route("api/v{version:apiVersion}/devices")]
[ApiController]
public class DeviceController : ControllerBase
{
    private readonly ICommandPublisher _commandPublisher;

    private readonly IQueryPublisher _queryPublisher;

    public DeviceController(IQueryPublisher queryPublisher, ICommandPublisher commandPublisher)
    {
        _queryPublisher = queryPublisher;
        _commandPublisher = commandPublisher;
    }

    /// <summary>
    /// Gets all devices.
    /// </summary>
    /// <returns>List of all devices.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeviceListResponse))]
    public async Task<IActionResult> GetAllDevicesAsync()
    {
        IEnumerable<DeviceResponse> devices = (await _queryPublisher.ExecuteAsync(new DeviceListQuery())).ToDtos();
        return Ok(new DeviceListResponse { Devices = devices });
    }

    /// <summary>
    /// Gets device by installationId.
    /// </summary>
    /// <returns>List of all devices.</returns>
    [HttpGet("{installationId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeviceResponse))]
    public async Task<IActionResult> GetDeviceByIdAsync(Guid installationId)
    {
        DeviceResponse device = (await _queryPublisher.ExecuteAsync(new DeviceByIdQuery(installationId))).ToDtoNotNull();
        return Ok(device);
    }

    /// <summary>
    /// Create device.
    /// </summary>
    /// <returns>device.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeviceResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateDeviceAsync([FromBody] DeviceRequest deviceRequest)
    {
        DeviceResponse device = (await _commandPublisher.ExecuteAsync(
            new CreateDeviceCommand(
                deviceRequest.InstallationId,
                deviceRequest.Model,
                deviceRequest.Name,
                deviceRequest.Platform.ToModelNotNull(),
                deviceRequest.DeviceToken))).ToDto();

        return Ok(device);
    }

    /// <summary>
    /// Update device.
    /// </summary>
    /// <returns>device.</returns>
    [HttpPut("{installationId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeviceResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateDeviceAsync([FromBody] DeviceUpdateRequest deviceUpdateRequest, Guid installationId)
    {
        DeviceResponse device = (await _commandPublisher.ExecuteAsync(new UpdateDeviceCommand(installationId, deviceUpdateRequest.Name, deviceUpdateRequest.DeviceToken))).ToDtoNotNull();
        return Ok(device);
    }

    /// <summary>
    /// Delete device.
    /// </summary>
    /// <returns>no content.</returns>
    [HttpDelete("{installationId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeviceResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteDeviceAsync(Guid installationId)
    {
        await _commandPublisher.ExecuteAsync(new DeleteDeviceCommand(installationId));
        return NoContent();
    }
}
