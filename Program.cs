using Microsoft.EntityFrameworkCore;
using MediCore.Data;

var builder = WebApplication.CreateBuilder(args);

// Registrar SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

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



// --- BLOQUE DE CREACIÓN Y SEMILLA DE DATOS ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    
    // Aplica migraciones automáticamente y crea la tabla si no existe
    context.Database.Migrate();
    if (!context.Patients.Any())
    {
        context.Patients.AddRange(
            new MediCore.Models.Patient { Name = "Carlos Pérez", DocumentNumber = "11223344", Age = 32, Gender = "Masculino", Diagnosis = "Hipertensión arterial controlada" },
            new MediCore.Models.Patient { Name = "María Gómez", DocumentNumber = "55667788", Age = 28, Gender = "Femenino", Diagnosis = "Control prenatal regular" },
            new MediCore.Models.Patient { Name = "Juan Rodríguez", DocumentNumber = "99001122", Age = 45, Gender = "Masculino", Diagnosis = "Diabetes Mellitus Tipo 2" }
        );
        context.SaveChanges();
    }
}
// ---------------------------------------------

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
