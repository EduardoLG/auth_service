using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Domain.Entitis;

public class UserProfile 
{
    [Key]
    [MaxLength(16)]
    public string Id {get; set;} = string.Empty;

    [Required]
    [MaxLength(16)]
    [ForeignKey(nameof(User))]
    public string UserId {get; set;} = string.Empty;

    public string ProfilePictureUrl {get; set;}  = string.Empty;
    public string? ProfilePicture { get; set; }
   public string? Phone { get; set; }
    public string Bio {get; set;}  = string.Empty;
    public DateTime DateOfBirth {get; set;} 
    public User User {get; set;} = null!;
    
}
/*
| id (PK, 16)      | userId (FK, 16)  | ProfilePictureUrl              | Bio                | DateOfBirth         |
|------------------|------------------|--------------------------------|--------------------|---------------------|
| "UP-12345678901" | "USR-9876543210" | "https://cdn.com/u1.jpg"       | "Software Engineer"| 1990-01-01 00:00:00 |
| "UP-09876543210" | "USR-1234567890" | "https://cdn.com/u2.jpg"       | "UI/UX Designer"   | 1995-05-15 00:00:00 |
*/

