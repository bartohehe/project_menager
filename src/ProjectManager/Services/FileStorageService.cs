using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace ProjectManager.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IMongoDbService _mongoDb;

    public FileStorageService(IMongoDbService mongoDb)
    {
        _mongoDb = mongoDb;
    }

    public async Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, string projectId, string category)
    {
        var bucket = _mongoDb.GetGridFSBucket();

        var options = new GridFSUploadOptions
        {
            Metadata = new BsonDocument
            {
                { "projectId", projectId },
                { "contentType", contentType },
                { "category", category }
            }
        };

        var fileId = await bucket.UploadFromStreamAsync(fileName, stream, options);
        return fileId.ToString();
    }

    public async Task<byte[]> DownloadFileAsync(string fileId)
    {
        var bucket = _mongoDb.GetGridFSBucket();
        var objectId = new ObjectId(fileId);
        return await bucket.DownloadAsBytesAsync(objectId);
    }

    public async Task DeleteFileAsync(string fileId)
    {
        var bucket = _mongoDb.GetGridFSBucket();
        var objectId = new ObjectId(fileId);
        await bucket.DeleteAsync(objectId);
    }

    public async Task DeleteAllForProjectAsync(string projectId)
    {
        var bucket = _mongoDb.GetGridFSBucket();
        var db = _mongoDb.GetDatabase();
        var filesCollection = db.GetCollection<BsonDocument>("fs.files");

        var filter = Builders<BsonDocument>.Filter.Eq("metadata.projectId", projectId);
        var files = await filesCollection.Find(filter).ToListAsync();

        foreach (var file in files)
        {
            var id = file["_id"].AsObjectId;
            await bucket.DeleteAsync(id);
        }
    }
}
