using Microsoft.AspNetCore.Http;
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
      //  [Required]
        public string? Email { get; set; }
// [Required]
        public string ?Password { get; set; }
       

        public string? Age { get; set; }

        public string? Role { get; set; }

        [NotMapped]
        public IFormFile? ProfileImage { get; set; }

        [Column("ProfileImage")]
        public string? ProfileImagePath { get; set; }


        [NotMapped]
        public bool RememberMe { get; set; }

        public DateTime? ExpireDate { get; set; }
        [NotMapped]
        public string? OldPassword { get; set; }

        public string? TokenResetPassword { get; set; }

        public DateTime? TokenResetExpireDate { get; set; }

        





    }
}
