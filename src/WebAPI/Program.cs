using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;
using TicketMaster.Application;
using TicketMaster.Application.Services;
using TicketMaster.Infrastructure;
using TicketMaster.Infrastructure.Identity;
using TicketMaster.Infrastructure.Services;
using WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCors();
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddRateLimiter(x =>
{
    x.AddFixedWindowLimiter("fixed", cfg =>
    {
        cfg.QueueLimit = 100;
        cfg.Window = TimeSpan.FromSeconds(1);
        cfg.PermitLimit = 100;
        cfg.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

    });
});
        
var app = builder.Build();

await app.SeedIdentityAsync();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapDefaultEndpoints();
app.UseCors(options =>
{
    options.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();
app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers().RequireRateLimiting("fixed");


app.Run();
