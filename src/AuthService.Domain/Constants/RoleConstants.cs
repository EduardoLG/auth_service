//nombre del espacio de trabajo
namespace AuthService.Domain.Constants;

//constantes del dominio
public class RoleConstants
{
    public const string ADMIN_ROLE = "ADMIN_ROLE";
    public const string USER_ROLE = "USER_ROLE";
    public static readonly string[] AllowedRoles = { ADMIN_ROLE, USER_ROLE };
}
