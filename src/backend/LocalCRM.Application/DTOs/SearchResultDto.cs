namespace LocalCRM.Application.DTOs;

public class SearchResultDto
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
}
