using AutoMapper;
using commands;
using host_api.Repositories;
using host_api.Requests;
using host_api.Services;
using Microsoft.AspNetCore.Mvc;
using view_models;

namespace host_api.Controllers;

[ApiController]
[Route("[controller]")]
public class HostController : ControllerBase
{
    private readonly ICommandHandler _commandHandler;
    private readonly IMapper _mapper;
    private readonly IViewModelRepository _viewModelRepository;

    public HostController(ICommandHandler commandHandler, IMapper mapper, IViewModelRepository viewModelRepository)
    {
        _commandHandler = commandHandler;
        _mapper = mapper;
        _viewModelRepository = viewModelRepository;
    }

    [HttpGet("GetAllHosts", Name = "Get all hosts")]
    public IActionResult GetAllHosts()
    {
        var allHosts = _viewModelRepository.GetAll<AllHostsViewModel>();

        return new JsonResult(allHosts);
    }

    [HttpPost("SignUp", Name = "New hosts sign up here")]
    public IActionResult SignUpNewHost(SignUpNewHostRequest request)
    {
        var command = _mapper.Map<SignUpNewHostCommand>(request);

        _commandHandler.Dispatch(command);

        return new JsonResult(new { });
    }
}