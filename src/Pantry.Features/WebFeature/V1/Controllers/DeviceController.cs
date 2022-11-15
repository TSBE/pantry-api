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
    /// Gets device by id.
    /// </summary>
    /// <returns>List of all devices.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeviceListResponse))]
    public async Task<IActionResult> GetDeviceByIdAsync(long id)
    {
        DeviceResponse device = (await _queryPublisher.ExecuteAsync(new DeviceByIdQuery(id))).ToDtoNotNull();
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
                deviceRequest.DeviceToken,
                deviceRequest.InstallationId,
                deviceRequest.Model,
                deviceRequest.Name,
                deviceRequest.Platform.ToModelNotNull()))).ToDto();

        return Ok(device);
    }
}
