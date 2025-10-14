using GoogleRuta.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5262");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.WebHost.UseKestrel(serverOptions =>
{
    // Escuchar en localhost para acceso local
    serverOptions.Listen(System.Net.IPAddress.Loopback, 5001);

    // ⭐ ESTA ES LA LÍNEA CLAVE PARA EL ACCESO REMOTO ⭐
    // Escuchar en todas las interfaces para acceso remoto
    serverOptions.Listen(System.Net.IPAddress.Any, 5001);
});

//services sql
var connectionString = builder.Configuration.GetConnectionString("ConexionSqlServer");
// Coloca un punto de interrupción (breakpoint) en la línea siguiente
Console.WriteLine($"Connection String: {connectionString}"); // Esto imprimirá el valor en la ventana de Salida/Output
builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSqlServer")));
//services
builder.Services.AddScoped<GoogleRuta.Services.Interfaces.IProjectService, GoogleRuta.Services.ProjectService>();
builder.Services.AddScoped<GoogleRuta.Services.Interfaces.IElementService, GoogleRuta.Services.ElementService>();
builder.Services.AddScoped<GoogleRuta.Services.Interfaces.IColorTracesService, GoogleRuta.Services.ColorTracesServices>();
builder.Services.AddScoped<GoogleRuta.Services.Interfaces.IRouterService, GoogleRuta.Services.RouterServices>();
builder.Services.AddScoped<GoogleRuta.Services.Interfaces.ISwitchService, GoogleRuta.Services.SwitchService>();

builder.Services.AddScoped<GoogleRuta.Services.Interfaces.IElfaService, GoogleRuta.Services.ElfaService>();


builder.Services.AddScoped<GoogleRuta.Services.Interfaces.IOdfService, GoogleRuta.Services.OdfService>();
builder.Services.AddScoped<GoogleRuta.Services.Interfaces.IOdlService, GoogleRuta.Services.OdlService>();
builder.Services.AddScoped<GoogleRuta.Services.Interfaces.IConnectionTelecomService, GoogleRuta.Services.ConnectionTelecomService>();

//builder.Services.AddScoped<GoogleRuta.Services.Interfaces.IDeviceConnectionService, GoogleRuta.Services.DeviceConnectionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Agrega esta línea aquí para habilitar el servicio de archivos estáticos.
// Esta línea es la que permite servir archivos de la carpeta wwwroot,
// incluso si se agregan en tiempo de ejecución.
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
