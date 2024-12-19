using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace lab7.Models
{
    public class bookorder
    {
        public int Id { get; set; }
        public string custname { get; set; }
        public int total { get; set; }
        [BindProperty, DataType(DataType.Date)]
        public DateTime orderdate { get; set; }
    }
}
