using ApiGateway.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure JWT Bearer Authentication to use Firebase's parameters
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Firebase", options =>
{
    options.Authority = "https://securetoken.google.com/interactiveflix-ae062";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "https://securetoken.google.com/interactiveflix-ae062",
        ValidateAudience = true,
        ValidAudience = "interactiveflix-ae062",
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()    // Allow all origins
                  .AllowAnyHeader()    // Allow any header
                  .AllowAnyMethod();   // Allow any method
                                       // .AllowCredentials(); // Don't use this if AllowAnyOrigin is used
        });
});

var app = builder.Build();

// Configure the app to use middleware
app.UseRouting();

// Use CORS before other middlewares
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

// Add Role Validation Middleware
app.UseMiddleware<RoleValidationMiddleware>();

app.UseOcelot().Wait();

app.Run();
