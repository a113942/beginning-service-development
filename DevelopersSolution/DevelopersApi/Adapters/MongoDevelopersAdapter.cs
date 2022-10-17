using DevelopersApi.Domain;
using MongoDB.Driver;

namespace DevelopersApi.Adapters;

public class MongoDevelopersAdapter
{

    public IMongoCollection<DeveloperEntity> Developers { get; private set; }

    public MongoDevelopersAdapter(string connnectionString)
    {
        var client = new MongoClient(connnectionString); // connect to the database
        var database = client.GetDatabase("developers"); // the database is developers
        Developers = database.GetCollection<DeveloperEntity>("devs"); // in there is a collection (table) devs

    }
}
