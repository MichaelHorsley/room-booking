using Microsoft.AspNetCore.Mvc;

namespace host_api.Controllers;

[ApiController]
[Route("[controller]")]
public class RoomController : ControllerBase
{
    private readonly ILogger<RoomController> _logger;

    public RoomController(ILogger<RoomController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetAllRooms")]
    public JsonResult GetAllRooms()
    {
        _logger.LogInformation("Testing");

        return new JsonResult(new {});
    }

    [HttpPost("RegisterNewRoom", Name = "Register new room")]
    public JsonResult RegisterNewRoom()
    {
        return new JsonResult(new { });
    }
}