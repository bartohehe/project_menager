using MongoDB.Bson.Serialization.Attributes;

namespace ProjectManager.Models;

public class ProjectAttachment
{
    [BsonElement("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("fileName")]
    public string FileName { get; set; } = string.Empty;

    [BsonElement("fileId")]
    public string FileId { get; set; } = string.Empty;

    [BsonElement("contentType")]
    public string ContentType { get; set; } = string.Empty;

    [BsonElement("fileSizeBytes")]
    public long FileSizeBytes { get; set; }

    [BsonElement("uploadedAt")]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
