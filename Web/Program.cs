using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CleanRestApi.Data;
using CleanRestApi.Application.Interfaces;
using CleanRestApi.Repositories;
using CleanRestApi.Services;
using CleanRestApi.Application.Handlers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL Database Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Clean Architecture Dependencies - Infrastructure Layer
builder.Services.AddScoped<IUserRepository, DatabaseUserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Application Layer - Handlers
builder.Services.AddScoped<CreateUserCommandHandler>();
builder.Services.AddScoped<UpdateUserCommandHandler>();
builder.Services.AddScoped<GetUserByIdQueryHandler>();
builder.Services.AddScoped<GetAllUsersQueryHandler>();
builder.Services.AddScoped<AuthenticateUserQueryHandler>();
builder.Services.AddScoped<DeleteUserCommandHandler>();

// Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
// AddSwaggerGen: Configures Swagger UI for testing API endpoints
builder.Services.AddSwaggerGen(c =>
{
    // Basic API information displayed in Swagger UI
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My RestAPI", Version = "v1" });

    // JWT Authentication Integration with Swagger UI
    // AddSecurityDefinition: Defines how authentication works in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,          // Token goes in HTTP header
        Description = "Please enter a valid JWT token with Bearer prefix", // User instruction
        Name = "Authorization",                 // Header name
        Type = SecuritySchemeType.ApiKey,      // Type of authentication
        Scheme = "Bearer"                       // Authentication scheme name
    });

    // AddSecurityRequirement: Makes Swagger UI show "Authorize" button
    // This allows users to input their JWT token for testing protected endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme //ref pointer to the auth method defined above
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme, //tells swagger about the type of auth
                    Id = "Bearer"                       // Must match the security definition above
                }
            },
            new string[] {}  // No specific scopes required such as  "read" "write" "admin" permissions etc 
        }
    });
});


// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? 
                    throw new InvalidOperationException("JWT SecretKey not found in configuration")))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();