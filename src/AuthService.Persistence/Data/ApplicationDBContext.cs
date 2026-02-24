using AuthService.Domain.Entitis;

using Microsoft.EntityFrameworkCore;
 
namespace AuthService.Persistence.Data;
 
public class ApplicationDbContext : DbContext

{

    // MÉTODO CONSTRUCTOR

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)

    {

    }
 
    // REPRESENTACIÓN DE TABLAS EN EL MODELO

    public DbSet<User> Users { get; set; }

    public DbSet<UserProfile> UserProfiles { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<UserEmail> UserEmails { get; set; }

    public DbSet<UserPasswordReset> UserPasswordResets { get; set; }
 
 
    // CONVIERTE CAMEL CASE A SNAKE CASE

   protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // [Tu lógica de Snake Case se mantiene igual...]
    foreach (var entity in modelBuilder.Model.GetEntityTypes())
    {
        var tableName = entity.GetTableName();
        if (!string.IsNullOrEmpty(tableName)) entity.SetTableName(ToSnakeCase(tableName));

        foreach (var property in entity.GetProperties())
        {
            var columnName = property.GetColumnName();
            if (!string.IsNullOrEmpty(columnName)) property.SetColumnName(ToSnakeCase(columnName));
        }
    }

    modelBuilder.Entity<User>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.Email).IsUnique();
        entity.HasIndex(e => e.Username).IsUnique(); // 'n' minúscula como en tu clase

        // Relación 1:1 con UserProfile
        entity.HasOne(e => e.UserProfile)      // CAMBIADO: Antes decía .Profile
            .WithOne(p => p.User)
            .HasForeignKey<UserProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación 1:N con UserRoles
        entity.HasMany(e => e.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación 1:1 con UserEmail
        entity.HasOne(e => e.UserEmail)
            .WithOne(ue => ue.User)
            .HasForeignKey<UserEmail>(ue => ue.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación 1:1 con UserPasswordReset
        entity.HasOne(e => e.PasswordReset)    // CAMBIADO: Asegúrate que en User.cs sea PasswordReset
            .WithOne(upr => upr.User)
            .HasForeignKey<UserPasswordReset>(upr => upr.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<UserRole>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
    });

    modelBuilder.Entity<Role>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.Name).IsUnique();
    });
}


    // ------------------------------------------------------------

    // FUNCIÓN PARA CONFIGURAR EL NOMBRE DE DE CLASE A NOMBRE DE DB

    private static string ToSnakeCase(string input)

    {

        if (string.IsNullOrEmpty(input))

            return input;
 
        return string.Concat(

            input.Select((x, i) => i > 0 && char.IsUpper(x) 

                ? "_" + x.ToString().ToLower() 

                : x.ToString().ToLower())

        );

    }
 
}
 