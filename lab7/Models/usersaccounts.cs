using System.ComponentModel.DataAnnotations;

namespace lab7.Models
{
    public class usersaccounts
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string pass { get; set; }
        public string role { get; set; }
    }
}
