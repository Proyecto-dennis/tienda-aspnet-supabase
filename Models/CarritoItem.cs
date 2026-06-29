using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGIN.Models
{
    [Table("CarritoItems")]
    public class CarritoItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        public DateTime FechaAgregado { get; set; } = DateTime.Now;

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; }
    }
}