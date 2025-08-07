using GoogleRuta.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5262");

// Add services to the container.
builder.Services.AddControllersWithViews();
//services sql
var connectionString = builder.Configuration.GetConnectionString("ConexionSqlServer");
// Coloca un punto de interrupción (breakpoint) en la línea siguiente
Console.WriteLine($"Connection String: {connectionString}"); // Esto imprimirá el valor en la ventana de Salida/Output
builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSqlServer")));
//services
builder.Services.AddScoped<GoogleRuta.Services.Interfaces.IProjectService, GoogleRuta.Services.ProjectService>();
builder.Services.AddScoped<GoogleRuta.Services.Interfaces.IElementService, GoogleRuta.Services.ElementService>();
builder.Services.AddScoped<GoogleRuta.Services.Interfaces.IColorTracesService,GoogleRuta.Services.ColorTracesServices>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
