using AppIntgration.Services;
using AppIntgration.API.Middleware;
using AppIntgration.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ????? ???????
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ????? ??????? ???????
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDataService, DataService>();

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

var app = builder.Build();

// ????? Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowMauiApp");
app.UseHttpsRedirection();

// ????? Middleware ???? ????????
app.UseMiddleware<AuthenticationMiddleware>();

app.UseAuthorization();
app.MapControllers();

app.Run();