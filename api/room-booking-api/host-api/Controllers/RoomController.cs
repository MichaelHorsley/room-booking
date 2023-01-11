using AutoMapper;
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
    private readonly IMapper _mapper;

    public RoomController(ILogger<RoomController> logger, ICommandHandler commandHandler, IMapper mapper)
    {
        _logger = logger;
        _commandHandler = commandHandler;
        _mapper = mapper;
    }

    [HttpPost("RegisterNewRoom", Name = "Register new room")]
    public IActionResult RegisterNewRoom(RegisterNewRoomRequest request)
    {
        var command = _mapper.Map<RegisterNewRoomCommand>(request);

        _commandHandler.Dispatch(command);

        return new JsonResult(new { });
    }
}