using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using OpenIddict.Validation.AspNetCore;
using System.Data;
using System.Reflection;
using Yggdrasil.AuthServer.Constants;
using Yggdrasil.AuthServer.Data;
using Yggdrasil.AuthServer.Infrastructure;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Authentication Server",
        Version = "v1",
        Description = "Descripción de mi API",
        Contact = new OpenApiContact
        {
            Name = "Sergio Modolell",
            Email = "sergio.modolell@email.com"
        }
    });
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Password = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri("/connect/token", UriKind.Relative),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID" },
                    { "profile", "Profile" },
                    { "email", "Email" },
                    { "api", "API Access" }
                }
            }
        }
    });
    c.AddSecurityRequirement(document => new() { [new OpenApiSecuritySchemeReference("oauth2", document)] =  { "openid", "profile", "email", "api" } });

    //c.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "oauth2"
    //            }
    //        },
    //        new[] { "openid", "profile", "email", "api" }
    //    }
    //});
    
});


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(connectionString);
    options.UseOpenIddict();
});

// Configuración de Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;

    // User settings
    options.User.RequireUniqueEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();



builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

// Configuración de OpenIddict
builder.Services.AddOpenIddict()
    .AddCore(opt => opt.UseEntityFrameworkCore()
    .UseDbContext<ApplicationDbContext>())
    .AddServer(opt =>
    {

        opt.SetAuthorizationEndpointUris("/connect/authorize")
            .SetTokenEndpointUris("/connect/token")
            .SetConfigurationEndpointUris("/.well-known/openid-configuration")
            .SetUserInfoEndpointUris("/connect/userinfo")
            .SetEndSessionEndpointUris("/connect/logout");

        // Flujos permitidos
        opt.AllowPasswordFlow()
            .AllowClientCredentialsFlow()
            .AllowRefreshTokenFlow()
            .AllowAuthorizationCodeFlow()
            .RequireProofKeyForCodeExchange();


        opt.AcceptAnonymousClients();
        opt.RegisterScopes(
            Scopes.OpenId,
            Scopes.Profile,
            Scopes.Email,
            Scopes.Roles,
            "api");
        opt.Configure(options => options.TokenValidationParameters.ValidAudience = "yggdrasil_app");



        // Certificados (en producción usa certificados reales)
        if (builder.Environment.IsDevelopment())
        {
            opt.AddDevelopmentEncryptionCertificate()
                   .AddDevelopmentSigningCertificate();
        }
        else
        {
            //opt.AddEncryptionCertificate("...")
            //       .AddSigningCertificate("...");
        }


        // Configuración ASP.NET Core
        opt.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableTokenEndpointPassthrough()
            .EnableUserInfoEndpointPassthrough()
            .EnableEndSessionEndpointPassthrough();

        opt.DisableAccessTokenEncryption();



    }).AddValidation(options =>
    {
        // Este esquema se usa para validar tokens JWT (Bearer)
        options.SetIssuer("https://localhost:7297/"); // Tu URL de Auth Server

        // Habilita el uso del entorno ASP.NET Core
        options.UseAspNetCore();

        // Crucial: Permite que el validador confíe en tokens emitidos por el mismo servidor/proceso
        options.UseLocalServer();

        options.AddAudiences("yggdrasil_app");
    });



builder.Services.AddControllers();


builder.Services.AddRazorPages();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

}
// Swagger debe estar antes de UseRouting y UseAuthentication
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Server v1");
    c.OAuthClientId("swagger-ui");
    c.OAuthClientSecret("swagger-ui-secret"); // Configura esto según tu cliente OpenID
    c.OAuthUsePkce();
});
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(builder => builder
    .WithOrigins("https://localhost:7212") // Puerto de Blazor
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseAntiforgery();
app.UseStaticFiles();


app.MapControllers();
app.MapRazorPages();




using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await OpenIddictSeed.SeedAsync(services);
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var rolesClass = typeof(Roles);
    var roleFields = rolesClass.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                              .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string));

    foreach (var field in roleFields)
    {
        var roleName = field.Name;

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    string adminEmail = "admin@admin.dev";
    string adminPassword = "Sergio123456%"; // En producción usa una contraseña segura

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);

    }

    foreach (var field in roleFields)
    {
        var roleName = field.Name;
        if (!await userManager.IsInRoleAsync(adminUser, roleName))
        {
            await userManager.AddToRoleAsync(adminUser, roleName);
        }
    }

}

app.Run();
