using AppIntgration.Services;
using AppIntgration.API.Middleware;
using AppIntgration.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ????? ??????? ????????
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ????? Memory Cache ??????
builder.Services.AddMemoryCache();

// ????? ??????? ???????
builder.Services.AddScoped<IAuthService, AuthService>();
// ??????? DataService ?? EnhancedDataService
builder.Services.AddSingleton<IDataService, EnhancedDataService>();

// ????? CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMauiApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ????? HttpClient ??????? ???????? (???????)
builder.Services.AddHttpClient();

var app = builder.Build();

// ????? Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowMauiApp");
app.UseHttpsRedirection();

// ????? Authentication Middleware
app.UseMiddleware<AuthenticationMiddleware>();

app.UseAuthorization();
app.MapControllers();

// ????? endpoint ??? ???????
app.MapGet("/health", () => new {
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0"
});

app.Run();