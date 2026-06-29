using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LOGIN.Models;
using LOGIN.Data;

namespace LOGIN.Controllers
{
    public class PedidosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PedidosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ver mis pedidos
        public async Task<IActionResult> MisPedidos()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
            {
                return RedirectToAction("Login", "Account");
            }

            var pedidos = await _context.Pedidos
                .Include(p => p.PedidoDetalles)
                .ThenInclude(d => d.Producto)
                .Where(p => p.UsuarioId == int.Parse(usuarioId))
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            return View(pedidos);
        }

        // Ver detalle de un pedido
        public async Task<IActionResult> DetallePedido(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.PedidoDetalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }
    }
}