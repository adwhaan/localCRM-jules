namespace LocalCRM.Domain.Entities;

public class RefreshToken
{
    public int RefreshTokenId { get; set; }
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public string SessionId { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public int? ReplacedByTokenId { get; set; }
    public DateTime? ReuseDetectedAt { get; set; }
    public string? CreatedByIp { get; set; }
    public string? UserAgent { get; set; }
}
