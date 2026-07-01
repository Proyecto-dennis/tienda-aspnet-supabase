using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LOGIN.Models;
using LOGIN.Data;

namespace LOGIN.Controllers
{
    public class CarritoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarritoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ver el carrito
        public async Task<IActionResult> Index()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
            {
                return RedirectToAction("Login", "Account");
            }

            var carrito = await _context.CarritoItems
                .Include(c => c.Producto)
                .Where(c => c.UsuarioId == int.Parse(usuarioId))
                .ToListAsync();

            return View(carrito);
        }

        // Agregar producto al carrito
        public async Task<IActionResult> Agregar(int id)
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
            {
                return RedirectToAction("Login", "Account");
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null || producto.Cantidad <= 0)
            {
                TempData["Error"] = "Producto no disponible";
                return RedirectToAction("Index", "Tienda");
            }

            // Buscar si ya existe en el carrito
            var carritoItem = await _context.CarritoItems
                .FirstOrDefaultAsync(c => c.UsuarioId == int.Parse(usuarioId) && c.ProductoId == id);

            if (carritoItem != null)
            {
                carritoItem.Cantidad++;
            }
            else
            {
                carritoItem = new CarritoItem
                {
                    UsuarioId = int.Parse(usuarioId),
                    ProductoId = id,
                    Cantidad = 1,
                    FechaAgregado = DateTime.UtcNow
                };
                _context.CarritoItems.Add(carritoItem);
            }

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Producto agregado al carrito";
            return RedirectToAction("Index", "Tienda");
        }

        // Eliminar producto del carrito
        public async Task<IActionResult> Eliminar(int id)
        {
            var carritoItem = await _context.CarritoItems.FindAsync(id);
            if (carritoItem != null)
            {
                _context.CarritoItems.Remove(carritoItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Vaciar el carrito
        public async Task<IActionResult> Vaciar()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (!string.IsNullOrEmpty(usuarioId))
            {
                var carrito = await _context.CarritoItems
                    .Where(c => c.UsuarioId == int.Parse(usuarioId))
                    .ToListAsync();
                _context.CarritoItems.RemoveRange(carrito);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Confirmar pedido
        // Ir al formulario de pago
        public async Task<IActionResult> ConfirmarPedido()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
                return RedirectToAction("Login", "Account");

            var carrito = await _context.CarritoItems
                .Include(c => c.Producto)
                .Where(c => c.UsuarioId == int.Parse(usuarioId))
                .ToListAsync();

            if (!carrito.Any())
            {
                TempData["Error"] = "El carrito está vacío";
                return RedirectToAction("Index");
            }

            ViewBag.Total = carrito.Sum(c => c.Cantidad * c.Producto.Precio);
            return View("Pago");
        }

        // Mostrar formulario de pago
        [HttpGet]
        public async Task<IActionResult> Pago()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
                return RedirectToAction("Login", "Account");

            var carrito = await _context.CarritoItems
                .Include(c => c.Producto)
                .Where(c => c.UsuarioId == int.Parse(usuarioId))
                .ToListAsync();

            ViewBag.Total = carrito.Sum(c => c.Cantidad * c.Producto.Precio);
            return View();
        }

        // Realizar compra simulada
        [HttpPost]
        public async Task<IActionResult> RealizarCompra()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
                return RedirectToAction("Login", "Account");

            var carrito = await _context.CarritoItems
                .Include(c => c.Producto)
                .Where(c => c.UsuarioId == int.Parse(usuarioId))
                .ToListAsync();

            if (!carrito.Any())
            {
                TempData["Error"] = "El carrito está vacío";
                return RedirectToAction("Index");
            }


            var pedido = new Pedido
            {
                UsuarioId = int.Parse(usuarioId),
                Total = carrito.Sum(c => c.Cantidad * c.Producto.Precio),
                Estado = 0,
                FechaPedido = DateTime.UtcNow,
                MetodoPago = "Visa"
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();


            foreach (var item in carrito)
            {
                if (item.Producto == null || item.Producto.Cantidad < item.Cantidad)
                {
                    TempData["Error"] = $"No hay stock suficiente para {item.Producto?.Nombre}";
                    return RedirectToAction("Index");
                }

                var detalle = new PedidoDetalle
                {
                    PedidoId = pedido.Id,
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Producto.Precio
                };

                _context.PedidoDetalles.Add(detalle);


                item.Producto.Cantidad -= item.Cantidad;
                _context.Productos.Update(item.Producto);
            }


            _context.CarritoItems.RemoveRange(carrito);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "¡Compra realizada correctamente!";
            return RedirectToAction("MisPedidos", "Pedidos");
        }
    }
}