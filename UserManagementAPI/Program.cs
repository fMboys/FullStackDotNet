var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseRequestResponseLogging();
app.UseGlobalExceptionHandler();
app.UseTokenValidation();

// In-memory data store
var users = new List<User>
{
    new()
    {
        Id = 1,
        FirstName = "John",
        LastName = "Doe",
        Email = "john.doe@email.com"
    },
    new()
    {
        Id = 2,
        FirstName = "Jane",
        LastName = "Smith",
        Email = "jane.smith@email.com"
    }
};

#region GET

// Get all users
app.MapGet("/users", () =>
{
    return Results.Ok(users);
});

// Get user by Id
app.MapGet("/users/{id:int}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);

    return user is null
        ? Results.NotFound($"User with Id {id} not found.")
        : Results.Ok(user);
});

#endregion

#region POST

// Create new user
app.MapPost("/users", (User user) =>
{
    var nextId = users.Any()
        ? users.Max(x => x.Id) + 1
        : 1;

    user.Id = nextId;

    users.Add(user);

    return Results.Created($"/users/{user.Id}", user);
});

#endregion

#region PUT

// Update existing user
app.MapPut("/users/{id:int}", (int id, User updatedUser) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);

    if (user is null)
        return Results.NotFound($"User with Id {id} not found.");

    user.FirstName = updatedUser.FirstName;
    user.LastName = updatedUser.LastName;
    user.Email = updatedUser.Email;

    return Results.Ok(user);
});

#endregion

#region DELETE

// Delete user
app.MapDelete("/users/{id:int}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);

    if (user is null)
        return Results.NotFound($"User with Id {id} not found.");

    users.Remove(user);

    return Results.NoContent();
});

#endregion

app.Run();
