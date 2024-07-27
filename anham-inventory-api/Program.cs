using anham_inventory_api.Context.DataSeeding;
using anham_inventory_api.Helpers.JwtConfigurations;
using anham_inventory_api.Services.AuthenticationServices;
using anham_inventory_api.Services.RoleManagementServices;
using anham_inventory_api.Services.UserManagementServices;
using anham_inventory_api;
using anham_inventory_api.Helpers;
using anham_inventory_api.Helpers.JWTConfiguration;
using anham_inventory_api.Models.UserManagemnt;
using anham_inventory_api.Services.EmailHelperService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
// configue dbContext class with connection string
builder.Services.AddDbContext<AnhamInventoryContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions => { sqlOptions.EnableRetryOnFailure(); });
}, ServiceLifetime.Transient);

// configure identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(
    option =>
    {
        option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        option.Lockout.MaxFailedAccessAttempts = 5;
    }).AddEntityFrameworkStores<AnhamInventoryContext>().AddDefaultTokenProviders();

// cnfigure role base ploicy for authorization
builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
});

builder.Services.Configure<IdentityOptions>(options =>
{
    // Remove all password requirements
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 0; // Set to 0 to remove minimum length requirement
});

builder.Services.AddControllers();

// configure jwt related
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
       opt.TokenLifespan = TimeSpan.FromHours(2));
var jwtTokenConfig = builder.Configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>();
builder.Services.AddSingleton(jwtTokenConfig);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtTokenConfig.Issuer,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
        ValidAudience = jwtTokenConfig.Audience,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
    x.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    //This is to generate the Default UI of Swagger Documentation  
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0.0",
        Title = "Hodhod API",
        Description = "Rest API'S Build of Hodhod By NKU TECHNOLOGIES"
    });
    // To Enable authorization using Swagger (JWT)  
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                            new string[] {}

                    }
                });
});

// CUSTOME SERVICES STARTS
builder.Services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
builder.Services.AddHostedService<JwtRefreshTokenCache>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

// seeding data service
builder.Services.AddTransient<DataSeeder>();

// email service
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// data seeder for empty database
if (args.Length == 1 && args[0].ToLower() == "dataseed")
    SeedData(app);

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory?.CreateScope())
    {
        var service = scope?.ServiceProvider.GetService<DataSeeder>();
        service?.Seed();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(builder =>
    {
        builder.Run(async context =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var error = context.Features.Get<IExceptionHandlerFeature>();
            if (error != null)
            {
                context.Response.AddApplicationError(error.Error.Message);
                await context.Response.WriteAsync(error.Error.Message);
            }
        });
    });
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((hosts) => true));
app.UseRouting();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
