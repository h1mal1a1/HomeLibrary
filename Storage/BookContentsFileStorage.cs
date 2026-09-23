namespace HomeLibrary.Storage;

public static class BookContentsFileStorage
{
    private static readonly string FolderPath = Path.Combine("Data", "BookContents");

    public static string Save(string xml, string? existingFilePath = null)
    {
        var directory = Path.Combine(AppContext.BaseDirectory, FolderPath);

        Directory.CreateDirectory(directory);

        var fileName = string.IsNullOrWhiteSpace(existingFilePath)
            ? $"{Guid.NewGuid()}.xml"
            : Path.GetFileName(existingFilePath);

        var fullPath = Path.Combine(directory, fileName);

        File.WriteAllText(fullPath, xml);

        return Path.Combine(FolderPath, fileName);
    }
}