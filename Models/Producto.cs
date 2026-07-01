using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGIN.Models
{
    [Table("Productos")]
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre")]
        public string? Nombre { get; set; }

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(0, int.MaxValue)]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue)]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }

        // NUEVO: Categoría del repuesto
        [Display(Name = "Categoría")]
        public string? Categoria { get; set; }

        // NUEVO: Marca del repuesto
        [Display(Name = "Marca")]
        public string? Marca { get; set; }

        // NUEVO: Modelo de auto compatible
        [Display(Name = "Modelo compatible")]
        public string? ModeloAuto { get; set; }

        //NUEVO: Imagenes
        [Display(Name = "Imagen")]
        public string? ImagenUrl { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}