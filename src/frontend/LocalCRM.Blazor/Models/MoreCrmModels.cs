namespace LocalCRM.Blazor.Models;

public class InteractionDto
{
    public int InteractionId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string InteractionType { get; set; } = string.Empty;
    public DateOnly InteractionDate { get; set; }
    public string State { get; set; } = string.Empty;
}

public class NoteDto
{
    public int NoteId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class DocumentDto
{
    public int DocumentId { get; set; }
    public string DocumentRef { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
}
