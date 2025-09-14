using System.Threading.Tasks;
using MapBackend.Model;
using Microsoft.EntityFrameworkCore;

namespace MapBackend.Endpoints
{
    public static class ProgramEndpoints
    {
        public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("users");

            group.MapGet("/", async (UserDb db) => await db.Users.ToListAsync());

            group.MapGet("/{id}", async (int id, UserDb db) =>
            await db.Users.FindAsync(id) is User user ? Results.Ok(user) : Results.NotFound())
            .WithName("GetUser");

            group.MapPost("/", async (User user, UserDb db) =>
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return Results.CreatedAtRoute("GetUser", new { id = user.Id }, user);

            });

            group.MapPut("/{id}", async (int id, User inputUser, UserDb db) =>
            {
                var user = await db.Users.FindAsync(id);
                if (user is null) return Results.NotFound();

                user.Username = inputUser.Username;
                user.Email = inputUser.Email;
                user.PasswordHash = inputUser.PasswordHash;
                user.CreatedAt = inputUser.CreatedAt;
                user.UpdatedAt = inputUser.UpdatedAt;

                await db.SaveChangesAsync();
                return Results.NoContent();
            }
            );

            group.MapDelete("/{id}", async (int id, UserDb db) =>
            {
                if (await db.Users.FindAsync(id) is User user)
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                    return Results.NoContent();
                }
                return Results.NotFound();
            }
            );
            return group;
        }
    }
}