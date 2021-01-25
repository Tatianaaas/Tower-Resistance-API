using System.ComponentModel.DataAnnotations;

namespace JogoApi.Model.Users
{
    public class RegisterModel
    {

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }


    }
}