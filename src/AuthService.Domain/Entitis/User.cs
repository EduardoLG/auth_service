using System.ComponentModel.DataAnnotations;
using AuthService.Domain.Entitis;

namespace AuthService.Domain.Entitis;

public class User
{
    [Key]
    [MaxLength(16)]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(25)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es requerido")]
    [MaxLength(25)]
    public string Surname { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    public bool Status { get; set; } = true;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    // Relaciones de navegación
    public UserProfile? UserProfile { get; set; }       // opcional
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public UserPasswordReset? PasswordReset { get; set; } // opcional
    public UserEmail? UserEmail { get; set; }      // opcional
}
