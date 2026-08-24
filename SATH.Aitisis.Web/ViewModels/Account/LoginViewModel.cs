using System.ComponentModel.DataAnnotations;

namespace SATH.Aitisis.Web.ViewModels.Account
{
    public class LoginViewModel
    {
        // I need this ViewModel class because the login page needs only these three properties
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
