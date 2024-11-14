using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace Project123.Dto
{
    public class AdminModel
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string ?Password { get; set; }

        public string? Role { get; set; }
        [NotMapped]
        public bool RememberMe { get; set; }

    }
}
