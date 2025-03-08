using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Mappings;
using NZWalks.API.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using Serilog;
using NZWalks.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Write the log to the console --- SERILOG CONFIGURATION
var logger = new LoggerConfiguration()
    .WriteTo.Console() // write the same piece of information to two different sources
    .WriteTo.File("Logs/NzWalks_Log.txt", rollingInterval: RollingInterval.Minute) // Check the logs per day
    .MinimumLevel.Warning()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);



builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add Authorization header when we are making a call -> To use authentication in Swagger.
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "NZ Walks API",
        Version = "v1"
    });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Injecting DbContext class
builder.Services.AddDbContext<NZWalksDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksConnectionString")));

builder.Services.AddDbContext<NZWalksAuthDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksAuthConnectionString")));

// Inject repository. Whenever i want to use IRegionRepository, i want to pass the concrete implementation of SQLRegionRepository
/*  Whenever the application asks for an IRegionRepository, the DI container will provide an instance of SQLRegionRepository, 
 * which is the concrete implementation of the IRegionRepository interface.
 * 
 * AddScoped -> "Every time an HTTP request is made to the server, a new instance of SQLRegionRepository will be created for that request.
 * If the IRegionRepository is requested multiple times during the same request, the same instance of SQLRegionRepository will be used
 */
builder.Services.AddScoped<IRegionRepository, SQLRegionRepository>();
builder.Services.AddScoped<IWalkRepository, SQLWalkRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IImageRepository, LocalImageRepository>();

// Scans for all the mappings that it could find in this file, when it needs it, it will be injected in the controller
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("NZWalks")
    .AddEntityFrameworkStores<NZWalksAuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{ // options for the password we want to configure, how many digits, how many uppercase-lowercase
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1; // at least one unique character required
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // read it from appsettings.json file
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

// Add authentication into the middleware pipeline
app.UseAuthentication();

app.UseAuthorization();


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath = "/Images"
    // https://localhost:1234/Images routing us to this pyhsical folder we have provided
});

app.MapControllers();

app.Run();
