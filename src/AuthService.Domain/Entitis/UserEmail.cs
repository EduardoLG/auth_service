public class UserEmail
{
    [Key]
    [MaxLength(16)]
    public string id { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public bool EmailVerified { get; set; } = false;

    [MaxLength(256)]
    public string? EmailVerificationToken { get; set; } = string.Empty;

    public DateTime? EmailVerifiedTokenExpire { get; set; }

    [Required]
    public User User { get; set; } = null!;
}