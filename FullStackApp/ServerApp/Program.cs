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

var app = builder.Build();

//app.UseCors("BlazorPolicy");
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

app.MapGet("/api/productlist", () =>

{

    return new[]

    {

        new { Id = 1, Name = "Laptop", Price = 1200.50, Stock = 25 },

        new { Id = 2, Name = "Headphones", Price = 50.00, Stock = 100 }

    };

});

app.Run();