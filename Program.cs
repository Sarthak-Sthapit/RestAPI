using Microsoft.EntityFrameworkCore;                    // Entity Framework Core for database operations
using RestAPI.Data;                                     
using RestAPI.Repositories;                             
using Microsoft.AspNetCore.Authentication.JwtBearer;   // JWT Bearer authentication middleware
using Microsoft.IdentityModel.Tokens;                  // Token validation and security key classes
using System.Text;                                      // Text encoding for converting strings to bytes
using RestAPI.Services;                               
using Microsoft.OpenApi.Models;                        // Swagger/OpenAPI models for API documentation

var builder = WebApplication.CreateBuilder(args);

// Add controllers to Dependency Injection container
// This enables MVC pattern and allows controllers to be discovered
builder.Services.AddControllers();

// Configure Entity Framework with PostgreSQL
// AddDbContext<T>: Registers your DbContext class with the DI container
// - Creates a new instance for each request (Scoped lifetime by default)
// - Makes database operations available throughout your application
// UseNpgsql: Tells Entity Framework to use PostgreSQL as the database provider
// GetConnectionString: Retrieves connection string from appsettings.json under "ConnectionStrings:DefaultConnection"
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection Registration:
// AddScoped<Interface, Implementation>: Creates new instance per HTTP request (recommended for database operations)
// This maps the IUserRepository interface to DatabaseUserRepository implementation
// When a controller needs IUserRepository, it will get DatabaseUserRepository instance
builder.Services.AddScoped<IUserRepository, DatabaseUserRepository>();
builder.Services.AddScoped<JwtService>(); // Register JWT service for token creation/validation

// JWT Authentication Configuration
// These values come from appsettings.json under "Jwt" section
// ?? throw: If configuration values are missing, throw an exception (fail-fast approach)
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new ArgumentNullException("Jwt:SecretKey not found in configuration");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new ArgumentNullException("Jwt:Issuer not found in configuration");

// AddAuthentication: Registers authentication services with the DI container
// JwtBearerDefaults.AuthenticationScheme: Sets JWT Bearer as the default authentication method
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // TokenValidationParameters: Defines how incoming JWT tokens should be validated
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // ValidateIssuerSigningKey: Ensures the token was signed with our secret key
            ValidateIssuerSigningKey = true,
            // IssuerSigningKey: The key used to verify token signatures (must match token creation key)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            
            // ValidateIssuer: Checks if token came from our application
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            
            // ValidateAudience: Checks if token is intended for our application
            ValidateAudience = true,
            ValidAudience = jwtIssuer, // Using same as issuer for simplicity
            
            // ValidateLifetime: Ensures token hasn't expired
            ValidateLifetime = true
        };
    });

// Swagger Configuration for API Documentation
// AddEndpointsApiExplorer: Discovers API endpoints for Swagger documentation
builder.Services.AddEndpointsApiExplorer();

// AddSwaggerGen: Configures Swagger UI for testing API endpoints
builder.Services.AddSwaggerGen(c =>
{
    // Basic API information displayed in Swagger UI
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "My RestAPI", Version = "v2" });

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

// Build the WebApplication from the configured builder
// This creates the actual application instance that will handle HTTP requests
var app = builder.Build();

// Development-only middleware
// Only enable Swagger UI in development to prevent exposing API documentation in production
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // Generates OpenAPI specification
    app.UseSwaggerUI();     // Provides interactive API documentation UI
}

// HTTP Pipeline Middleware (ORDER MATTERS!)
app.UseHttpsRedirection();  // Redirects HTTP requests to HTTPS for security

// Authentication Pipeline:
// UseAuthentication: Checks if incoming requests have valid JWT tokens
// - Parses Authorization header
// - Validates token signature, expiration, issuer, etc.
// - Sets User identity if token is valid
app.UseAuthentication();

// UseAuthorization: Enforces access control based on user identity
// - Checks if authenticated user has required permissions
// - Works with [Authorize] attributes on controllers/actions
app.UseAuthorization();

// Controller Mapping (CRITICAL!)
// MapControllers: Scans Controllers/ folder and maps controller actions to HTTP routes
// - Looks for [HttpGet], [HttpPost], [Route] attributes
// - Without this, controllers won't respond to HTTP requests
app.MapControllers();

// Start the application and begin listening for HTTP requests
app.Run();