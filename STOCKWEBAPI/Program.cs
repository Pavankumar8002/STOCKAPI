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
using STOCKWEBAPI.RepositoryInterface.RootStackx;
using STOCKWEBAPI.Repository.RootStackx;
using STOCKWEBAPI.ServiceInterface.RootStackx;
using STOCKWEBAPI.Service.RootStackx;

var builder = WebApplication.CreateBuilder(args);

// Controllers & Swagger
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

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

// ✅ CORS Policy — for React Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ✅ JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "STOCKWEBAPI",
            ValidAudience = "STOCKWEBAPI_CLIENT",
            IssuerSigningKey = new SymmetricSecurityKey(
                Convert.FromBase64String("Vfdz0zTcW1qqIt9UgyEu+LdJ4HEavPKBzBkw9wwHjZo=")),
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
builder.Services.AddScoped<IEnqueryRepo, EnqueryRepo>();
builder.Services.AddScoped<IEnqueryService, EnqueryService>();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock API V1");
        options.RoutePrefix = string.Empty;
    });
    app.MapGet("/", context =>
    {
        context.Response.Redirect("/swagger");
        return Task.CompletedTask;
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\": \"An unexpected error occurred.\"}");
    });
});

// ✅ Middleware Order is Important
app.UseStaticFiles();
app.UseRouting();

// ✅ CORS must come BEFORE auth
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// MVC Controller Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
