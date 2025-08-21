using OfficeOpenXml;
using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_DSWI.Services;
using Proyecto_Planilla_Entidades;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


var apiBaseUrl = builder.Configuration["ApiService:URL"];

#region Injeccion de Dependencias
builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddScoped<IEstadoCivilService, EstadoCivilService>();
builder.Services.AddScoped<IGeneroService, GeneroService>();
builder.Services.AddScoped<ISistemaPensionService, SistemaPensionService>();
builder.Services.AddScoped<ISituacionTrabajadorService, SituacionTrabajadorService>();
builder.Services.AddScoped<ITipoDocumentoService,TipoDocumentoService>();
builder.Services.AddScoped<IParametroService,ParametroService>();
builder.Services.AddScoped<IAsistenciaService,AsistenciaService>();
builder.Services.AddScoped<ITrabajadorService, TrabajadorService>();
#endregion

var app = builder.Build();
// Configuración de la licencia EPPlus (debe ser lo primero)
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
   pattern: "{controller=Home}/{action=Index}/{id?}");
    // pattern: "{controller=Asistencia}/{action=CargaAsistencia}");

app.Run();
