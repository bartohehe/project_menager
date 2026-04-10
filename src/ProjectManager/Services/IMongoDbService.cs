using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace ProjectManager.Services;

public interface IMongoDbService
{
    bool IsConnected { get; }
    IMongoDatabase GetDatabase();
    IGridFSBucket GetGridFSBucket();
    Task<bool> TestConnectionAsync(string connectionString);
    void Initialize(string connectionString);
}
