using System.ComponentModel.DataAnnotations;

namespace lab7.Models
{
    public class book
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string title { get; set; }
        
        [Required]
        public decimal price { get; set; }
        
        public string? imgfile { get; set; }
    }
}
