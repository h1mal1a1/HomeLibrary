namespace HomeLibrary.Models;

public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public int PublicationYear { get; set; }

    public string ContentsXml { get; set; } = string.Empty;

    public string? ContentsFilePath { get; set; }

    public string? Description { get; set; }
}