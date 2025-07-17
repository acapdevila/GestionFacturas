using GestionFacturas.AccesoDatosSql;
using GestionFacturas.Aplicacion;
using GestionFacturas.Web.Pages.Facturas;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using GestionFacturas.AccesoDatosSql.Repos;
using System.Globalization;
using GestionFacturas.Web.Framework;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("es-ES");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Sets the default scheme to cookies
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.AccessDeniedPath = "/seguridad/acceso/accesodenegado";
        options.LoginPath = "/seguridad/acceso/entrar";
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // Crear otras policy
});
var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
builder.Services.AddScoped(m=> 
    new SqlDb(connectionString!));


// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute(ListaGestionFacturasModel.NombrePagina, "");

    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToFolder("/seguridad/acceso");
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.WriteIndented = true;
});
var mailSettings = new MailSettings();
builder.Configuration.GetSection("MailSettings").Bind(mailSettings);
builder.Services.AddSingleton(mailSettings);
builder.Services.AddScoped<IServicioEmail, ServicioEmailMailKid>();
builder.Services.AddScoped<CambiarEstadoFacturaServicio>();
builder.Services.AddScoped<CambiarEstadoFacturaRepo>();


var app = builder.Build();
app.SincronizarBaseDatos();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute();

await app.RunAsync();
