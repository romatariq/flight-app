using System.Text;
using App.BLL;
using App.Contracts.BLL;
using App.Contracts.DAL;
using App.DAL.EF;
using App.DAL.EF.Seeding;
using App.Domain.Identity;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FlightInfoCollector;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApp;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IAppUOW, AppUOW>();
builder.Services.AddScoped<IAppBLL, AppBLL>();

builder.Services.AddScoped<IDataCollector, DataCollectorWebScraper>();

builder.Services.AddIdentity<AppUser, AppRole>(
        options => options.SignIn.RequireConfirmedAccount = false)
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services
    .AddAuthentication()
    .AddCookie(options => { options.SlidingExpiration = true; })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = builder.Configuration.GetValue<string>("JWT:Issuer")!,
            ValidAudience = builder.Configuration.GetValue<string>("JWT:Audience")!,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:Key")!)),
            ClockSkew = TimeSpan.Zero,
        };
    });


builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsAllowAll", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});


var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    // in case of no explicit version
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

apiVersioningBuilder.AddApiExplorer(options =>
{
    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
    // note: the specified format code will format the version as "'v'major[.minor][-status]"
    options.GroupNameFormat = "'v'VVV";

    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
    // can also be used to control the format of the API version in route templates
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();


builder.Services.AddAutoMapper(
    typeof(App.Mappers.AutoMapperConfigs.PublicDTOConfig),
    typeof(App.Mappers.AutoMapperConfigs.BLLConfig)
);


var app = builder.Build();

// Set up db
SetupDb(app, app.Environment, app.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("CorsAllowAll");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


app.UseSwagger();
app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName
            );
        }
    }
);


app.Run();




void SetupDb(IApplicationBuilder webApp, IWebHostEnvironment appEnvironment, IConfiguration appConfiguration)
{
    using var serviceScope = webApp.ApplicationServices
        .GetRequiredService<IServiceScopeFactory>()
        .CreateScope();
    using var context = serviceScope.ServiceProvider.GetService<AppDbContext>();


    if (context == null)
    {
        throw new ApplicationException("Problem in services. Can't initialize Application DB Context");
    }

    using var userManager = serviceScope.ServiceProvider.GetService<UserManager<AppUser>>();
    using var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<AppRole>>();
    
    if (userManager == null || roleManager == null)
    {
        throw new ApplicationException("Problem in services. Can't initialize UserManager or RoleManager");
    }
    
    var logger = serviceScope.ServiceProvider.GetService<ILogger<IApplicationBuilder>>();
    if (logger == null)
    {
        throw new ApplicationException("Problem in services. Can't initialize logger");
    }

    if (context.Database.ProviderName != null && context.Database.ProviderName.Contains("InMemory"))
    {
        return;
    }

    // wait for db connection
    var startedAt = DateTime.UtcNow;
    var isDbConnectable = context.Database.CanConnectAsync().Result;
    while (!isDbConnectable)
    {
        isDbConnectable = context.Database.CanConnectAsync().Result;
        if (!isDbConnectable && (DateTime.UtcNow - startedAt).Seconds > 10)
        {
            throw new ApplicationException("Could not connect to database");
        }
    }
    

    if (appConfiguration.GetValue<bool>("InitializeData:DropDatabase"))
    {
        logger.LogWarning("Dropping database");
        DbInitializer.DropDatabase(context);
    }

    if (appConfiguration.GetValue<bool>("InitializeData:MigrateDatabase"))
    {
        logger.LogInformation("Migrating database");
        DbInitializer.MigrateDatabase(context);
    }

    if (appConfiguration.GetValue<bool>("InitializeData:SeedIdentity"))
    {
        logger.LogInformation("Seeding identity");
        DbInitializer.SeedIdentity(userManager, roleManager);
    }

    if (appConfiguration.GetValue<bool>("InitializeData:SeedData"))
    {
        logger.LogInformation("Seeding initial app data");
        DbInitializer.InitializeDb(context, appEnvironment.WebRootPath);
    }

}

/// <summary>
/// To create custom webApp for testing.
/// </summary>
public partial class Program
{
}