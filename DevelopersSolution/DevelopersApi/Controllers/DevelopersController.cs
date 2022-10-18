using DevelopersApi.Adapters;
using DevelopersApi.Domain;
using DevelopersApi.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DevelopersApi.Controllers;

public class DevelopersController : ControllerBase
{
    private readonly MongoDevelopersAdapter _mongoAdapter;

    public DevelopersController(MongoDevelopersAdapter mongoAdapter)
    {
        _mongoAdapter = mongoAdapter;
    }

    [HttpPut("/on-call-developer")]
    public async Task<ActionResult> AssignOnCallDeveloper([FromBody] DeveloperSummaryModel request)
    {
        var objectId = ObjectId.Parse(request.Id);
        var whereIsOnCallFilter = Builders<DeveloperEntity>.Filter.Where(d => d.IsOnCallDeveloper == true);
        var updateToUnsetOnCallDeveloper = Builders<DeveloperEntity>.Update.Set(d => d.IsOnCallDeveloper, false);

        await _mongoAdapter.Developers.UpdateOneAsync(whereIsOnCallFilter, updateToUnsetOnCallDeveloper);

        var newIsOnCallFilter = Builders<DeveloperEntity>.Filter.Where(d => d.Id == objectId);
        var newIsOnCallUpdate = Builders<DeveloperEntity>.Update.Set(d => d.IsOnCallDeveloper, true);

        await _mongoAdapter.Developers.UpdateOneAsync(newIsOnCallFilter, newIsOnCallUpdate);

        return Accepted();
    }

    [HttpGet("/on-call-developer")]
    public async Task<ActionResult> GetOnCallDeveloper()
    {
        var response = await _mongoAdapter.Developers.AsQueryable()
            .Where(d => d.IsOnCallDeveloper == true)
            .Select(d => new DeveloperDetailsModel(d.Id.ToString(), d.FirstName, d.LastName, d.Phone, d.Email))
            .SingleOrDefaultAsync(); // 0 or 1. If it > 1 , errors out 

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
