using System.ComponentModel.DataAnnotations;

namespace vega.Models
{
    public class Contact
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(255)]
        public string ContactName { get; set; }

        [StringLength(255)]
        public string ContactEmail { get; set; }

        [Required]
        [StringLength(255)]
        public string ContactPhone { get; set; }
    }
}