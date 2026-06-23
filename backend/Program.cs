var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

var welcome = app.Configuration["AppSettings:WelcomeMessage"];
var version = app.Configuration["AppSettings:Version"];

app.Logger.LogInformation("Застосунок запущено. Середовище: {Env}", app.Environment.EnvironmentName);

app.MapGet("/", () =>
{
    app.Logger.LogInformation("Опрацювання запиту до головного ендпоінта");
    return $"{welcome} (версія {version})";
});

app.MapGet("/boom", () =>
{
    throw new Exception("Тестова помилка для перевірки Middleware");
});

app.Run();