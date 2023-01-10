using host_api.Requests;
using host_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace host_api.Controllers;

[ApiController]
[Route("[controller]")]
public class RoomController : ControllerBase
{
    private readonly ILogger<RoomController> _logger;
    private readonly ICommandHandler _commandHandler;

    public RoomController(ILogger<RoomController> logger, ICommandHandler commandHandler)
    {
        _logger = logger;
        _commandHandler = commandHandler;
    }

    [HttpGet(Name = "GetAllRooms")]
    public JsonResult GetAllRooms()
    {
        _logger.LogInformation("Testing");

        return new JsonResult(new {});
    }

    [HttpPost("RegisterNewRoom", Name = "Register new room")]
    public IActionResult RegisterNewRoom(RegisterNewRoomRequest request)
    {
        _commandHandler.Dispatch(new RegisterNewRoomCommand());

        return new JsonResult(new { });
    }
}