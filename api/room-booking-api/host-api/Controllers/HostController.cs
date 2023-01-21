using AutoMapper;
using commands;
using host_api.Requests;
using host_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace host_api.Controllers;

[ApiController]
[Route("[controller]")]
public class HostController : ControllerBase
{
    private readonly ICommandHandler _commandHandler;
    private readonly IMapper _mapper;

    public HostController(ICommandHandler commandHandler, IMapper mapper)
    {
        _commandHandler = commandHandler;
        _mapper = mapper;
    }

    [HttpPost("RegisterNewHost", Name = "Register new host")]
    public IActionResult RegisterNewHost(RegisterNewHostRequest request)
    {
        var command = _mapper.Map<RegisterNewHostCommand>(request);

        _commandHandler.Dispatch(command);

        return new JsonResult(new { });
    }
}