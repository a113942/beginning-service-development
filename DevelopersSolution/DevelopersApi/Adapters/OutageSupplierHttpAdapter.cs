using DevelopersApi.Controllers;
using DevelopersApi.Models;

namespace DevelopersApi.Adapters;

public class OutageSupplierHttpAdapter
{

    private readonly HttpClient _httpClient;

    public OutageSupplierHttpAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;

    }

    public async Task<List<ScheduledOutage>?> GetScheduledOutagesAsync()
    {
        var response = await _httpClient.GetAsync("/services/developers/outages");

        response.EnsureSuccessStatusCode(); // Weird. It will throw if the status code != (200-299)

        var outages = await response.Content.ReadFromJsonAsync<CollectionResponse<ScheduledOutage>>();
        if (outages != null)
        {
            return outages.Data;
        }
        return null;
    }
}