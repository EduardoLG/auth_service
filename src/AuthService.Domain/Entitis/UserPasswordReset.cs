namespace AuthService.Domain.Entitis;

public class UserPasswordReset
{
    [Key]
    [MaxLength(16)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [MaxLength(16)]
    public string UserId { get; set; } = string.Empty;

    [MaxLength(256)]
    public string? PasswordResetToken { get; set; } = string.Empty;

    public DateTime? PasswordResetTokenExpiry { get; set; }  

    [Required]
    public User User { get; set; } = null!; 
}

/*
| Id | UserId | PasswordResetToken | PasswordResetTokenExpiry | User (FK) |
|----|--------|----------------------|--------------------------|-----------|
| 1  | USR-123| abc123               | 2023-01-01 00:00:00      | USR-123   |
| 2  | USR-456| def456               | 2023-01-02 00:00:00      | USR-456   |
*/
