using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTO
{
    public class EditarAdmin
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}
