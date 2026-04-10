using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace ProjectManager.Services;

public class MongoDbService : IMongoDbService
{
    private readonly string _databaseName;
    private MongoClient? _client;
    private IMongoDatabase? _database;
    private IGridFSBucket? _gridFsBucket;

    public MongoDbService(IConfiguration configuration)
    {
        _databaseName = configuration["Database:Name"] ?? "ProjectManagerDb";
    }

    public bool IsConnected => _client is not null;

    public void Initialize(string connectionString)
    {
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
        settings.ConnectTimeout = TimeSpan.FromSeconds(5);

        _client = new MongoClient(settings);
        _database = _client.GetDatabase(_databaseName);
        _gridFsBucket = new GridFSBucket(_database);

        EnsureIndexes();
    }

    public async Task<bool> TestConnectionAsync(string connectionString)
    {
        try
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            settings.ConnectTimeout = TimeSpan.FromSeconds(5);

            var testClient = new MongoClient(settings);
            var db = testClient.GetDatabase(_databaseName);

            await db.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
            return true;
        }
        catch
        {
            return false;
        }
    }

    public IMongoDatabase GetDatabase()
    {
        return _database ?? throw new InvalidOperationException("Database not initialized. Configure connection first.");
    }

    public IGridFSBucket GetGridFSBucket()
    {
        return _gridFsBucket ?? throw new InvalidOperationException("Database not initialized. Configure connection first.");
    }

    private void EnsureIndexes()
    {
        if (_database is null) return;

        var projects = _database.GetCollection<BsonDocument>("projects");

        var indexModels = new[]
        {
            new CreateIndexModel<BsonDocument>(
                Builders<BsonDocument>.IndexKeys.Ascending("status")),
            new CreateIndexModel<BsonDocument>(
                Builders<BsonDocument>.IndexKeys.Ascending("category")),
            new CreateIndexModel<BsonDocument>(
                Builders<BsonDocument>.IndexKeys.Descending("updatedAt")),
            new CreateIndexModel<BsonDocument>(
                Builders<BsonDocument>.IndexKeys.Text("name").Text("shortDescription"))
        };

        projects.Indexes.CreateMany(indexModels);

        var categories = _database.GetCollection<BsonDocument>("categories");
        categories.Indexes.CreateOne(new CreateIndexModel<BsonDocument>(
            Builders<BsonDocument>.IndexKeys.Ascending("name"),
            new CreateIndexOptions { Unique = true }));
    }
}
