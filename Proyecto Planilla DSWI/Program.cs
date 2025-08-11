using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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
