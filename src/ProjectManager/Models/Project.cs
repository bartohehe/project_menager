using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectManager.Models;

public class Project
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("shortDescription")]
    public string ShortDescription { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public ProjectStatus Status { get; set; } = ProjectStatus.Planned;

    [BsonElement("price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Price { get; set; }

    [BsonElement("currency")]
    public string Currency { get; set; } = "PLN";

    [BsonElement("startDate")]
    public DateTime? StartDate { get; set; }

    [BsonElement("endDate")]
    public DateTime? EndDate { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("thumbnailFileId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ThumbnailFileId { get; set; }

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = [];

    [BsonElement("customSections")]
    public List<ProjectSection> CustomSections { get; set; } = [];

    [BsonElement("timelineEntries")]
    public List<TimelineEntry> TimelineEntries { get; set; } = [];

    [BsonElement("attachments")]
    public List<ProjectAttachment> Attachments { get; set; } = [];

    [BsonElement("cardOrder")]
    public List<string> CardOrder { get; set; } = [];
}
