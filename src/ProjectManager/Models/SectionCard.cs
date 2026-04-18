namespace ProjectManager.Models;

public enum SectionCardType { Description, Photos, Attachments, Custom }

public class SectionCard
{
    public SectionCardType Type    { get; init; }
    public string          Id      { get; init; } = string.Empty;
    public ProjectSection? Section { get; init; } // only for Custom type
}
