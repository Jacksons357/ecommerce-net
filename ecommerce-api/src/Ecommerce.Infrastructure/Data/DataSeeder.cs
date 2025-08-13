using Microsoft.EntityFrameworkCore;
using Ecommerce.Domain.Entities;
using BCrypt.Net;

namespace Ecommerce.Infrastructure.Data;

public static class DataSeeder
{
    // ID fixo para o usuário admin
    private static readonly Guid AdminId = new Guid("11111111-1111-1111-1111-111111111111");

    public static async Task SeedAsync(EcommerceDbContext context)
    {
        // Verificar se o usuário admin já existe
        if (await context.Usuarios.AnyAsync(u => u.Id == AdminId))
        {
            return; // Admin já foi criado
        }

        // Limpar usuário admin existente com email duplicado (caso exista)
        var usuarioExistente = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "admin@local");
        if (usuarioExistente != null && usuarioExistente.Id != AdminId)
        {
            context.Usuarios.Remove(usuarioExistente);
            await context.SaveChangesAsync();
        }

        // Criar usuário admin
        var admin = new Usuario
        {
            Id = AdminId,
            Nome = "Admin",
            Email = "admin@local",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123")
        };
        
        await context.Usuarios.AddAsync(admin);
        await context.SaveChangesAsync();
    }
}
