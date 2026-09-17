using GameVault.Api.Endpoints;
using GameVault.Api.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Configuración de ProblemDetails y Manejo Global de Excepciones (RFC 7807)
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Auto-descubrimiento y registro de todos los IEndpoint
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

// Pipeline de manejo de excepciones global (debe ir al inicio del pipeline)
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("MedBook API - Documentación Interactiva")
               .WithTheme(ScalarTheme.Moon)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

// Mapeo automático de todos los endpoints bajo el prefijo centralizado /api
app.MapEndpoints();

app.Run();
