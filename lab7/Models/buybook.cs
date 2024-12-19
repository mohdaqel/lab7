using System.ComponentModel.DataAnnotations;

namespace lab7.Models
{
    public class buybook
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int quant { get; set; }
    }
}
