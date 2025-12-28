using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

using STOCKWEBAPI.Helpers;

using STOCKWEBAPI.Repository.Login;
using STOCKWEBAPI.Repository.Users;
using STOCKWEBAPI.Repository.RootStackx;
using STOCKWEBAPI.Repository.Portfolio;

using STOCKWEBAPI.RepositoryInterface.Login;
using STOCKWEBAPI.RepositoryInterface.Users;
using STOCKWEBAPI.RepositoryInterface.RootStackx;
using STOCKWEBAPI.RepositoryInterface.Portfolio;

using STOCKWEBAPI.Service.Login;
using STOCKWEBAPI.Service.Users;
using STOCKWEBAPI.Service.RootStackx;
using STOCKWEBAPI.Service.Portfolio;

using STOCKWEBAPI.ServiceInterface.Login;
using STOCKWEBAPI.ServiceInterface.Users;
using STOCKWEBAPI.ServiceInterface.RootStackx;
using STOCKWEBAPI.ServiceInterface.Portfolio;

// =======================================================
// BUILDER (File Watchers DISABLED for Linux Hosting)
// =======================================================

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory()
});

// ❌ Remove default config watchers
builder.Configuration.Sources.Clear();

// ✅ Load configs WITHOUT reloadOnChange
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile(
        $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: false
    )
    .AddEnvironmentVariables();

// =======================================================
// SERVICES
// =======================================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// -------------------- Swagger --------------------
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Stock API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
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

// -------------------- CORS --------------------
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

// -------------------- JWT --------------------
var jwtKey = builder.Configuration["JWT_KEY"]
    ?? "Vfdz0zTcW1qqIt9UgyEu+LdJ4HEavPKBzBkw9wwHjZo="; // fallback (move to env in prod)

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
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
                Convert.FromBase64String(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<JwtTokenGenerator>();

// =======================================================
// DEPENDENCY INJECTION
// =======================================================

// Login
builder.Services.AddScoped<ILoginRepo, LoginRepo>();
builder.Services.AddScoped<ILoginService, LoginService>();

// Users
builder.Services.AddScoped<IUsersRepo, UsersRepo>();
builder.Services.AddScoped<IUsersService, UsersService>();

// RootStackx Enquiry
builder.Services.AddScoped<IEnqueryRepo, EnqueryRepo>();
builder.Services.AddScoped<IEnqueryService, EnqueryService>();
builder.Services.AddScoped<IEnquiryDetailsRepo, EnquiryDetailsRepo>();
builder.Services.AddScoped<IEnquiryDetailsService, EnquiryDetailsService>();

// Portfolio Enquiry
builder.Services.AddScoped<IPortfolioEnqueryRepo, PortfolioEnqueryRepo>();
builder.Services.AddScoped<IPortfolioEnqueryService, PortfolioEnqueryService>();

// =======================================================
// APP
// =======================================================

var app = builder.Build();

// -------------------- Error Handling --------------------
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(
            "{\"error\":\"An unexpected error occurred.\"}");
    });
});

// -------------------- Swagger --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock API V1");
        c.RoutePrefix = string.Empty;
    });
}

// -------------------- Middleware Order --------------------
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// -------------------- Routing --------------------
app.MapControllers();

app.Run();
