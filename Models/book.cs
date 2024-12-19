using System.ComponentModel.DataAnnotations;

namespace lab7.Models
{
    public class book
    {
        public int Id { get; set; }
        public string title { get; set; }
        public decimal price { get; set; }
        public string imgfile { get; set; }
    }
}
