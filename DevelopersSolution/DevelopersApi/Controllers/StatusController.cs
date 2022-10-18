using Microsoft.AspNetCore.Mvc;

namespace DevelopersApi.Controllers;

public class StatusController : ControllerBase
{
    [HttpGet("/status")]
    public ActionResult GetStatus()
    {
        var response = new StatusResponse("Looks Good, Captain!", DateTime.Now);
        return Ok(response);
    }
}

public record StatusResponse(string Status, DateTime whenChecked);
