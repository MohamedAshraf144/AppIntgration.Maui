using AppIntgration.Services;
using AppIntgration.API.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AppIntegration API",
        Version = "v1",
        Description = "System Monitor API with Bearer Token Authentication"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                      "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                      "Example: \"Bearer API_20250727005659_5ca27f31\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddMemoryCache();

// ????? ???????
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IDataService, EnhancedDataService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMauiApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHttpClient();

var app = builder.Build();

// ????? Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AppIntegration API V1");
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.DefaultModelsExpandDepth(-1); 
    });
}

app.UseCors("AllowMauiApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Health endpoint
app.MapGet("/health", () => new {
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0"
});

app.Run();