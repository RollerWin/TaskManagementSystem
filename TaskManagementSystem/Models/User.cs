using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManagementSystem.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        public string UserName { get; set; }

        public string? FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; } = string.Empty;

        public string Password { get; set; }

        public string Role { get; set; }

        // Навигационное свойство для связи с задачами
        public ICollection<TaskModel>? Tasks { get; set; }
    }


public static class UserEndpoints
{
	public static void MapUserEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/User").WithTags(nameof(User));

        group.MapGet("/", async (TaskManagementContext db) =>
        {
            return await db.Users.ToListAsync();
        })
        .WithName("GetAllUsers")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<User>, NotFound>> (int userid, TaskManagementContext db) =>
        {
            return await db.Users.AsNoTracking()
                .FirstOrDefaultAsync(model => model.UserID == userid)
                is User model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetUserById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int userid, User user, TaskManagementContext db) =>
        {
            var affected = await db.Users
                .Where(model => model.UserID == userid)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.UserID, user.UserID)
                  .SetProperty(m => m.FirstName, user.FirstName)
                  .SetProperty(m => m.LastName, user.LastName)
                  .SetProperty(m => m.Email, user.Email)
                  .SetProperty(m => m.Password, user.Password)
                  .SetProperty(m => m.Role, user.Role)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateUser")
        .WithOpenApi();

        group.MapPost("/", async (User user, TaskManagementContext db) =>
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/User/{user.UserID}",user);
        })
        .WithName("CreateUser")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int userid, TaskManagementContext db) =>
        {
            var affected = await db.Users
                .Where(model => model.UserID == userid)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteUser")
        .WithOpenApi();
    }
}}
