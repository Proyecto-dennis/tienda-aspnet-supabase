using Microsoft.EntityFrameworkCore;
using LOGIN.Data;
using LOGIN.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("ConexionSupabase")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("No se encontró la cadena de conexión. Configura ConnectionStrings:DefaultConnection o la variable DATABASE_URL.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    if (!db.Usuarios.Any(u => u.Email == "dennis@gmail.com"))
    {
        db.Usuarios.Add(new Usuario
        {
            Nombre = "Dennis Admin",
            Email = "dennis@gmail.com",
            Password = "12345",
            Edad = 20,
            Ciudad = "Bolivia",
            FechaRegistro = DateTime.UtcNow
        });
    }

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "Filtro de aceite", Descripcion = "Filtro compatible para mantenimiento", Cantidad = 20, Precio = 45, Categoria = "Filtros", Marca = "Genérico", ModeloAuto = "Universal", FechaRegistro = DateTime.UtcNow },
            new Producto { Nombre = "Pastillas de freno", Descripcion = "Juego de pastillas delanteras", Cantidad = 10, Precio = 180, Categoria = "Frenos", Marca = "Bosch", ModeloAuto = "Toyota", FechaRegistro = DateTime.UtcNow },
            new Producto { Nombre = "Bujía NGK", Descripcion = "Bujía para motor a gasolina", Cantidad = 30, Precio = 35, Categoria = "Motor", Marca = "NGK", ModeloAuto = "Universal", FechaRegistro = DateTime.UtcNow }
        );
    }

    db.SaveChanges();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllers();

app.Run();
