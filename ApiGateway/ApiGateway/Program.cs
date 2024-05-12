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
    options.Authority = "https://securetoken.google.com/interactiveflix";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "https://securetoken.google.com/interactiveflix",
        ValidateAudience = true,
        ValidAudience = "interactiveflix",
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Configure the app to use middleware
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseOcelot().Wait();

app.Run();
