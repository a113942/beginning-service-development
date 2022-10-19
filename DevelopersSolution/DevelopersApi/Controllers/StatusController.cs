using DevelopersApi.Adapters;
using Microsoft.AspNetCore.Mvc;

namespace DevelopersApi.Controllers;

public class StatusController : ControllerBase
{
    private readonly OutageSupplierHttpAdapter _outageSupplierHttpAdapter;

    public StatusController(OutageSupplierHttpAdapter outageSupplierHttpAdapter)
    {
        _outageSupplierHttpAdapter = outageSupplierHttpAdapter;
    }

    [HttpGet("/status")]
    public async Task<ActionResult> GetStatus()
    {
        List<ScheduledOutage>? outages;
        try
        {
            outages = await _outageSupplierHttpAdapter.GetScheduledOutagesAsync();
        }
        catch (Exception)
        {
            // log it, whatever
            outages = null;
        }
        //var outages = new List<ScheduledOutage>
        //{
        //    new ScheduledOutage(DateTime.Now.AddDays(3), DateTime.Now.AddDays(3).AddHours(1), "Holiday Party"),
        //    new ScheduledOutage(DateTime.Now.AddDays(4), DateTime.Now.AddDays(5).AddHours(1), "Staff Hanover Recovery"),
        //};
        var response = new StatusResponse("Looks Good, Captain!", DateTime.Now, outages);
        return Ok(response);
    }
}

public record StatusResponse(string Status, DateTime whenChecked, List<ScheduledOutage>? Outages);

public record ScheduledOutage(DateTime StartTime, DateTime EndTime, string Reason);
