using DevelopersApi.Models;
using Microsoft.AspNetCore.Mvc;
using DevelopersApi.Adapters;
using DevelopersApi.Domain;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;

namespace DevelopersApi.Controllers;

public class DevelopersController : ControllerBase
{
    private readonly MongoDevelopersAdapter _mongoAdapter;

    public DevelopersController(MongoDevelopersAdapter mongoAdapter)
    {
        _mongoAdapter = mongoAdapter;
    }

    // PUT /on-call-developer
    [HttpPut("/on-call-developer")]
    public async Task<ActionResult> AssignOnCallDeveloper([FromBody] DeveloperSummaryModel request)
    {
        // "Atomic Operation" (All or nothing) 
        // Done exactly one after the other, in ISOLATION from other calls to this.
        //   "Serializable"
        //   "ACID" - Atomic, Consistent, Isolated, Durable
        var objectId = ObjectId.Parse(request.Id);
        var whereIsOnCallFilter = Builders<DeveloperEntity>.Filter.Where(d => d.IsOnCallDeveloper == true);
        var updateToUnsetOnCallDeveloper = Builders<DeveloperEntity>.Update.Set(d => d.IsOnCallDeveloper, false);

        await _mongoAdapter.Developers.UpdateOneAsync(whereIsOnCallFilter, updateToUnsetOnCallDeveloper);
        // If there is one that is already on call, take them "off call"

        var newIsOnCallFilter = Builders<DeveloperEntity>.Filter.Where(d => d.Id == objectId);
        var newInOnCallUpdate = Builders<DeveloperEntity>.Update.Set(d => d.IsOnCallDeveloper, true);

        await _mongoAdapter.Developers.UpdateOneAsync(newIsOnCallFilter, newInOnCallUpdate);
        // we need to change the developer passed in here's oncall property to true and save it.
        return Accepted();
    }


    // GET /on-call-developer
    [HttpGet("/on-call-developer")]
    public async Task<ActionResult> GetOnCallDeveloper()
    {

        var response = await _mongoAdapter.Developers.AsQueryable()
            .Where(d => d.IsOnCallDeveloper == true)
            .Select(d => new DeveloperDetailsModel(d.Id.ToString(), d.FirstName, d.LastName, d.Phone, d.Email))
            .SingleOrDefaultAsync(); // 0 or 1. If it > 1, BLAMMO!

        return Ok(response); // 200 Status code.
    }

    [HttpGet("/developers")]
    public async Task<ActionResult> GetAllDevelopers()
    {
        var response = new CollectionResponse<DeveloperSummaryModel>();


        var data = _mongoAdapter.Developers.AsQueryable()
            .Select(d => new DeveloperSummaryModel(
                d.Id.ToString(), d.FirstName, d.LastName, d.Email));

        response.Data = await data.ToListAsync();


        return Ok(response);
    }

    [HttpPost("/developers")]
    public async Task<ActionResult> AddADeveloper([FromBody] DeveloperCreateModel request)
    {
        var developerToAdd = new DeveloperEntity
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            IsOnCallDeveloper = false
        };

        await _mongoAdapter.Developers.InsertOneAsync(developerToAdd);

        var response = new DeveloperSummaryModel(developerToAdd.Id.ToString(), developerToAdd.FirstName, developerToAdd.LastName, developerToAdd.Email);
        return StatusCode(201, response); // "Good. Ok. I created this.
    }
}