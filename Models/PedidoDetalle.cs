using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGIN.Models
{
    [Table("PedidoDetalles")]
    public class PedidoDetalle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal PrecioUnitario { get; set; }

        [ForeignKey("PedidoId")]
        public virtual Pedido? Pedido { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; }

        [NotMapped]
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}