using System.Text.Json;
using TodoApp.Api.Extensions;
using TodoApp.Core.Extensions;
using TodoApp.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services from other layers
builder.Services
    .AddCoreServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global error handling
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        var contextFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(contextFeature.Error, "An unhandled exception has occurred.");

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error."
            }));
        }
    });
});

// Enable CORS
app.UseCors("TodoAppPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure database is created and migrations are applied
await app.Services.MigrateDatabase();

app.Run();
