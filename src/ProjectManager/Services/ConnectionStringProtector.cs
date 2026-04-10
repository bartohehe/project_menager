using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjectManager.Services;

public class ConnectionStringProtector : IConnectionStringProtector
{
    private readonly string _filePath;

    public ConnectionStringProtector()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _filePath = Path.Combine(appData, "ProjectManager", "connection.dat");
    }

    public void Save(string connectionString)
    {
        var bytes = Encoding.UTF8.GetBytes(connectionString);
        var encrypted = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);

        var dir = Path.GetDirectoryName(_filePath)!;
        Directory.CreateDirectory(dir);
        File.WriteAllBytes(_filePath, encrypted);
    }

    public string? Load()
    {
        if (!File.Exists(_filePath))
            return null;

        var encrypted = File.ReadAllBytes(_filePath);
        var bytes = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(bytes);
    }

    public void Delete()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }
}
