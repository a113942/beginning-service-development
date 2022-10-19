namespace DevelopersApi.Models;



public record DeveloperDetailsModel(string Id, string FirstName, string LastName, string Phone, string EMail);

public record DeveloperSummaryModel(string Id, string FirstName, string LastName, string EMail);

public class CollectionResponse<T>
{
    public List<T> Data { get; set; } = new();
}

public record DeveloperCreateModel(string FirstName, string LastName, string Email, string Phone);