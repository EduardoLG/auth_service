using System.ComponentModel.DataAnnotations;
namespace AuthService.Domain.Entitis;

public class Role
{
    [Key]
    [MaxLength(16)]
    public string Id { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;

    //relacion con UserRole
    public ICollection<UserRole> UserRoles { get; set; } = [];
}

/*
| Id | Name  | Description                         |
|----|-------|-------------------------------------|
| 1  | Admin | Full system access and management.  |
| 2  | User  | Standard access to user features.   |
| 3  | Guest | Read-only access to public content. |
*/