using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApiTestedProject.Models
{
    public class Product
    {
        public Product(string name, int price)
        {
            Name= name;
            Price= price;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public int Price { get; set; }
    }
}
