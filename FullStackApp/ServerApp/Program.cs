var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5257")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

//app.UseCors("BlazorPolicy");
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

app.MapGet("/api/productlist", () => new
{
    products = new[]
    {
        new ProductDto(1, "Laptop", 1200.50m, 25, new CategoryDto(101, "Electronics")),
        new ProductDto(2, "Headphones", 50.00m, 100, new CategoryDto(102, "Accessories"))
    }
});

app.Run();

public record CategoryDto(int Id, string Name);
public record ProductDto(int Id, string Name, decimal Price, int Stock, CategoryDto Category);