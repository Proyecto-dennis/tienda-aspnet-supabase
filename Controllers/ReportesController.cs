using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LOGIN.Models;
using LOGIN.Data;

namespace LOGIN.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Reporte de ganancias
        public async Task<IActionResult> Ganancias()
        {
            // Verificar si hay usuario logueado
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Obtener todos los pedidos
            var pedidos = await _context.Pedidos
                .Include(p => p.PedidoDetalles)
                .ThenInclude(d => d.Producto)
                .ToListAsync();

            // Calcular estadísticas
            var totalPedidos = pedidos.Count;
            var totalGanancias = pedidos.Sum(p => p.Total);
            var pedidosPendientes = pedidos.Count(p => p.Estado == 0);
            var pedidosEntregados = pedidos.Count(p => p.Estado == 3);
            var pedidosCancelados = pedidos.Count(p => p.Estado == 4);

            // Productos más vendidos
            var productosMasVendidos = await _context.PedidoDetalles
                .Include(d => d.Producto)
                .GroupBy(d => d.ProductoId)
                .Select(g => new
                {
                    ProductoId = g.Key,
                    Nombre = g.First().Producto.Nombre,
                    TotalVendido = g.Sum(d => d.Cantidad),
                    TotalGanancia = g.Sum(d => d.Cantidad * d.PrecioUnitario)
                })
                .OrderByDescending(g => g.TotalVendido)
                .Take(5)
                .ToListAsync();

            // Categorías más vendidas
            var categoriasMasVendidas = await _context.PedidoDetalles
                .Include(d => d.Producto)
                .Where(d => d.Producto.Categoria != null)
                .GroupBy(d => d.Producto.Categoria)
                .Select(g => new
                {
                    Categoria = g.Key,
                    TotalVendido = g.Sum(d => d.Cantidad),
                    TotalGanancia = g.Sum(d => d.Cantidad * d.PrecioUnitario)
                })
                .OrderByDescending(g => g.TotalVendido)
                .ToListAsync();

            // Ganancias por mes
            var gananciasPorMes = await _context.Pedidos
                .Where(p => p.Estado == 3) // Solo entregados
                .GroupBy(p => new { p.FechaPedido.Year, p.FechaPedido.Month })
                .Select(g => new
                {
                    Año = g.Key.Year,
                    Mes = g.Key.Month,
                    Total = g.Sum(p => p.Total)
                })
                .OrderBy(g => g.Año)
                .ThenBy(g => g.Mes)
                .ToListAsync();

            ViewBag.TotalPedidos = totalPedidos;
            ViewBag.TotalGanancias = totalGanancias;
            ViewBag.PedidosPendientes = pedidosPendientes;
            ViewBag.PedidosEntregados = pedidosEntregados;
            ViewBag.PedidosCancelados = pedidosCancelados;
            ViewBag.ProductosMasVendidos = productosMasVendidos;
            ViewBag.CategoriasMasVendidas = categoriasMasVendidas;
            ViewBag.GananciasPorMes = gananciasPorMes;

            return View();
        }
    }
}
