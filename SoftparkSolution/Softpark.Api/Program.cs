using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Softpark.Api.Middlewares;
using Softpark.Application.Services;
using Softpark.Application.UseCases;
using Softpark.Infrastructure.Data;
using Softpark.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.WriteTo.Console();
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CriarUsuarioUseCase>();
builder.Services.AddScoped<AtualizarUsuarioUseCase>();
builder.Services.AddScoped<ObterUsuarioPorIdUseCase>();
builder.Services.AddScoped<ListarUsuariosUseCase>();

builder.Services.AddControllers();

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>() ?? new JwtSettings();

if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
{
    throw new InvalidOperationException("JwtSettings:SecretKey não foi configurado.");
}

var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Softpark API",
        Version = "v1",
        Description = "API do desafio técnico Softpark"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Informe apenas o token JWT."
    });

    options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();