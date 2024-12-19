using System.ComponentModel.DataAnnotations;

namespace lab7.Models
{
    public class bookorder
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string custname { get; set; }
        
        [Required]
        public DateTime orderdate { get; set; }
        
        [Required]
        public decimal total { get; set; }
    }
}
