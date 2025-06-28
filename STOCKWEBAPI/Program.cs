using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using STOCKWEBAPI.RepositoryInterface.Login;
using STOCKWEBAPI.Repository.Login;
using STOCKWEBAPI.ServiceInterface.Login;
using STOCKWEBAPI.Service.Login;
using STOCKWEBAPI.Repository.Users;
using STOCKWEBAPI.RepositoryInterface.Users;
using STOCKWEBAPI.Service.Users;
using STOCKWEBAPI.ServiceInterface.Users;
using STOCKWEBAPI.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Controllers and Swagger
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT Auth Support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Stock API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your JWT token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "STOCKWEBAPI",             // MUST match
            ValidAudience = "STOCKWEBAPI_CLIENT",
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String("Vfdz0zTcW1qqIt9UgyEu+LdJ4HEavPKBzBkw9wwHjZo=")),
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddAuthorization();
builder.Services.AddScoped<JwtTokenGenerator>();



// Dependency Injection
builder.Services.AddScoped<ILoginRepo, LoginRepo>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUsersRepo, UsersRepo>();

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock API V1");
    options.RoutePrefix = string.Empty;
});

// Standard Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // must be before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
