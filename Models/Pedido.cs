using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGIN.Models
{
    [Table("Pedidos")]
    public class Pedido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public DateTime FechaPedido { get; set; } = DateTime.UtcNow;

        [Required]
        public decimal Total { get; set; }

        public int Estado { get; set; } = 0;

        public string? DireccionEnvio { get; set; }

        public string? MetodoPago { get; set; }

        public string? NumeroReferencia { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        public virtual ICollection<PedidoDetalle>? PedidoDetalles { get; set; }

        [NotMapped]
        public string EstadoTexto
        {
            get
            {
                return Estado switch
                {
                    0 => "Pendiente",
                    1 => "Confirmado",
                    2 => "Enviado",
                    3 => "Entregado",
                    4 => "Cancelado",
                    _ => "Desconocido"
                };
            }
        }
    }
}