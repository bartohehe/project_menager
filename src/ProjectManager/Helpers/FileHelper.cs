using System.IO;

namespace ProjectManager.Helpers;

public static class FileHelper
{
    private const long MaxFileSize = 50 * 1024 * 1024; // 50 MB

    private static readonly Dictionary<string, byte[]> MagicBytes = new()
    {
        { ".jpg", [0xFF, 0xD8, 0xFF] },
        { ".jpeg", [0xFF, 0xD8, 0xFF] },
        { ".png", [0x89, 0x50, 0x4E, 0x47] },
        { ".gif", [0x47, 0x49, 0x46] },
        { ".bmp", [0x42, 0x4D] },
        { ".webp", [0x52, 0x49, 0x46, 0x46] },
        { ".pdf", [0x25, 0x50, 0x44, 0x46] },
        { ".zip", [0x50, 0x4B, 0x03, 0x04] },
        { ".docx", [0x50, 0x4B, 0x03, 0x04] },
        { ".xlsx", [0x50, 0x4B, 0x03, 0x04] },
    };

    private static readonly HashSet<string> AllowedExtensions =
    [
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg",
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
        ".txt", ".csv", ".rtf", ".odt", ".ods",
        ".zip", ".rar", ".7z",
        ".dwg", ".dxf", ".step", ".stl"
    ];

    private static readonly HashSet<string> ImageExtensions =
    [
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"
    ];

    public static (bool IsValid, string? Error) ValidateFile(string filePath)
    {
        var fileInfo = new FileInfo(filePath);

        if (!fileInfo.Exists)
            return (false, "Plik nie istnieje.");

        if (fileInfo.Length > MaxFileSize)
            return (false, $"Plik przekracza maksymalny rozmiar ({MaxFileSize / 1024 / 1024} MB).");

        var ext = fileInfo.Extension.ToLowerInvariant();

        if (!AllowedExtensions.Contains(ext))
            return (false, $"Rozszerzenie '{ext}' nie jest dozwolone.");

        if (MagicBytes.TryGetValue(ext, out var expected))
        {
            using var stream = File.OpenRead(filePath);
            var header = new byte[expected.Length];
            if (stream.Read(header, 0, header.Length) < header.Length)
                return (false, "Plik jest zbyt mały lub uszkodzony.");

            if (!header.AsSpan().StartsWith(expected))
                return (false, "Zawartość pliku nie odpowiada rozszerzeniu.");
        }

        return (true, null);
    }

    public static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Concat(fileName.Select(c => invalidChars.Contains(c) ? '_' : c));

        if (sanitized.Length > 200)
        {
            var ext = Path.GetExtension(sanitized);
            sanitized = sanitized[..(200 - ext.Length)] + ext;
        }

        return sanitized;
    }

    public static bool IsImageFile(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ImageExtensions.Contains(ext);
    }

    public static string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }

    public static string FormatFileSize(long bytes)
    {
        return bytes switch
        {
            < 1024 => $"{bytes} B",
            < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
            < 1024 * 1024 * 1024 => $"{bytes / (1024.0 * 1024):F1} MB",
            _ => $"{bytes / (1024.0 * 1024 * 1024):F2} GB"
        };
    }
}
