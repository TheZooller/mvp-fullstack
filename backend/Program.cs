using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using backend.Data;
using backend.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MVP Back-End API",
        Version = "v1",
        Description = "Документація REST API навчального MVP-проєкту."
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "MVP Back-End API v1");
    options.DocumentTitle = "MVP Back-End API";
});

var welcome = app.Configuration["AppSettings:WelcomeMessage"];
var version = app.Configuration["AppSettings:Version"];

app.MapGet("/", () => $"{welcome} (версія {version})");

app.MapGet("/products", async (AppDbContext db) => 
    await db.Products.ToListAsync())
    .WithTags("Products");

app.MapGet("/products/{id}", async (int id, AppDbContext db) => 
    await db.Products.FindAsync(id) is Product product
        ? Results.Ok(product)
        : Results.NotFound())
    .WithTags("Products");

app.MapPost("/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
})
.WithTags("Products");

app.MapPut("/products/{id}", async (int id, Product input, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    product.Name = input.Name;
    product.Price = input.Price;
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithTags("Products");

app.MapDelete("/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithTags("Products");

app.MapGet("/boom", () =>
{
    throw new Exception("Тестова помилка для перевірки Middleware");
});

app.Run();