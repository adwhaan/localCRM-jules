using System;

namespace LocalCRM.Application.DTOs
{
    public class EngagementDto
    {
        public int EngagementId { get; set; }
        public string? EngagementRef { get; set; }
        public string? Description { get; set; }
        public string? EngagementTags { get; set; }
        public string? Confidentiality { get; set; }
        public string EngagementStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class CreateEngagementDto
    {
        public string? EngagementRef { get; set; }
        public string? Description { get; set; }
        public string? EngagementTags { get; set; }
        public string? Confidentiality { get; set; }
        public string EngagementStatus { get; set; } = string.Empty;
    }

    public class UpdateEngagementDto : CreateEngagementDto { }

    public class NoteDto
    {
        public int NoteId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? Body { get; set; }
        public string Visibility { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class CreateNoteDto
    {
        public string Subject { get; set; } = string.Empty;
        public string? Body { get; set; }
        public string Visibility { get; set; } = string.Empty;
    }

    public class UpdateNoteDto : CreateNoteDto { }

    public class DocumentDto
    {
        public int DocumentId { get; set; }
        public string DocumentRef { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentUrl { get; set; } = string.Empty;
        public string Visibility { get; set; } = string.Empty;
        public string? Checksum { get; set; }
        public bool IsChecked { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class CreateDocumentDto
    {
        public string DocumentRef { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentUrl { get; set; } = string.Empty;
        public string Visibility { get; set; } = string.Empty;
        public string? Checksum { get; set; }
        public bool IsChecked { get; set; }
    }

    public class UpdateDocumentDto : CreateDocumentDto { }
}
