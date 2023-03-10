using AutoMapper;
using commands;
using host_api.Requests;
using host_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace host_api.Controllers;

[ApiController]
[Route("[controller]")]
public class RoomController : ControllerBase
{
    private readonly ICommandHandler _commandHandler;
    private readonly IMapper _mapper;

    public RoomController(ICommandHandler commandHandler, IMapper mapper)
    {
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