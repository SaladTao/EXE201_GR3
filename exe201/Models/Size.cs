using System.ComponentModel.DataAnnotations;

namespace exe201.Models
{
    public class Size
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
    }
} 