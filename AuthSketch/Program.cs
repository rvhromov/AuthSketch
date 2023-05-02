using AuthSketch.Extensions;
using AuthSketch.Middlewares;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddPersistence(builder.Configuration)
    .AddServices()
    .AddProviders()
    .AddAutoMapper(Assembly.GetExecutingAssembly())
    .AddAuthentication(builder.Configuration)
    .AddAuthorization(builder.Configuration)
    .AddHttpContextAccessor()
    .AddMiddlewares()
    .AddEmails(builder.Configuration)
    .AddHttpClient();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();