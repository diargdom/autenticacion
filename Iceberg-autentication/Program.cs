using Microsoft.AspNetCore.Authentication.Negotiate;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/************ Configuración de CORS ************/
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            //builder.AllowAnyOrigin()
            builder.WithOrigins("http://localhost:5173", "https://iceberg.cosmos.es.ftgroup:7195")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// Usar la política de CORS
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

/*************EndPoints*****************/
app.MapGet("/", () => "Backend autentication Up!");
app.MapGet("/windowsuser", (ClaimsPrincipal user) =>
{
    if (user.Identity.IsAuthenticated)
    {
        var username = user.Identity.Name.Split('\\').Last();
        return Results.Ok(new { username });
    }
    return Results.Unauthorized();
}).WithTags("Enpodint para autenticación de windows");

app.Run();
