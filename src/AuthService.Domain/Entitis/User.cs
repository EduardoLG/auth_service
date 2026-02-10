using System.ComponentModel.DataAnnotations;

namespace AuthService.Domain.   Entitis;

public class User
{
    [Key]
    [MaxLength(16)]
    public int Id { get; set; } = string.Empty;

    [Required (ErrorMessage = "El nombre es requerido")]
    [MaxLength(25)]
    public string Name { get; set; } = string.Empty;

    [Required (ErrorMessage = "El apellido es requerido")]
    [MaxLength(25)]
    public string Surname {get; set;} = string.Empty;

    [Required (ErrorMessage = "El nombre de usuario es requerido")]
    [MaxLength(50)]
    public string Username {get; set;} = string.Empty;

    [Required (ErrorMessage = "El correo es requerido")]
    [EmailAddress] //El valor de esta propiedad debe tener formato de correo electronico
    public string Email {get; set;} = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Password {get; set;} = string.Empty;

    public bool Status {get; set;} = true;

    [Required]
    public DateTime CreatedAt {get; set;}

    [Required]
    public DateTime UpdatedAt {get; set;}


    //Relaciones de navegacion solo dentro del codigo
    // Esto no altera la base de datos
    //Relacion con UserProfile
    public UserProfile Profile {get; set;} = null!; //El signo ! indica que esta propiedad no sera null
    //Relacion con UserRole
    public ICollection<UserRole> UserRoles { get; set; } = []; //El signo [] indica que esta propiedad sera una coleccion
    //Relacion con UserPasswordReset
    public UserPasswordReset PasswordReset {get; set;} = null!;
    //Relacion con UserEmail
    public UserEmail UserEmail {get; set;} = null!;
    
}
/*
+----+---------+-----------+----------+------------------------+----------+---------+---------------------+---------------------+
| Id | Name    | Surname   | Username | Email                  | Password | Status  | CreatedAt           | UpdatedAt           |
+----+---------+-----------+----------+------------------------+----------+---------+---------------------+---------------------+
| 1  | Eduardo | Rodriguez | eduardo  | eduardo@example.com    | ******** | Active  | 2023-10-27 10:00:00 | 2023-10-27 10:00:00 |
+----+---------+-----------+----------+------------------------+----------+---------+---------------------+---------------------+
*/

