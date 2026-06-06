using LocalCRM.Domain.Common;

namespace LocalCRM.Domain.Entities;

public class Note : BaseEntity
{
    public int NoteId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string Visibility { get; set; } = string.Empty;
}

public class Document : BaseEntity
{
    public int DocumentId { get; set; }
    public string DocumentRef { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
    public string? Checksum { get; set; }
    public bool IsChecked { get; set; }
}
