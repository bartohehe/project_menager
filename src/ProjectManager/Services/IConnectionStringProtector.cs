namespace ProjectManager.Services;

public interface IConnectionStringProtector
{
    void Save(string connectionString);
    string? Load();
    void Delete();
}
