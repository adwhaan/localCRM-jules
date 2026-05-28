using System;

namespace LocalCRM.Application.DTOs
{
    public class ContactDto
    {
        public int ContactId { get; set; }
        public string? ContactRef { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? LinkedinUrl { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? ContactTags { get; set; }
        public int Rating { get; set; }
        public string? Sex { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class CreateContactDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ContactRef { get; set; }
        public string? MiddleName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? LinkedinUrl { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? ContactTags { get; set; }
        public int Rating { get; set; }
        public string? Sex { get; set; }
    }

    public class UpdateContactDto : CreateContactDto { }
}
