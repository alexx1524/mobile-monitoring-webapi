using System.Reflection;
using FluentValidation.AspNetCore;
using Infotecs.Mobile.Monitoring.Data;
using Infotecs.Mobile.Monitoring.Data.Migrations;
using Infotecs.Mobile.Monitoring.WebApi.Validators;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddCors();
builder.Services
    .AddControllers()
    .AddFluentValidation(c => c.RegisterValidatorsFromAssemblyContaining<MonitoringDataValidator>());

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Infotecs Mobile Monitoring API", Version = "v1" });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Add Serilog
Logger? logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add Data services
builder.Services.AddDataServices(builder.Configuration);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.SerializeAsV2 = true;
    });
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.UseAuthorization();

app.MapControllers();

app.MigrateDatabase<Program>().Run();
