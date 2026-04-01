using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FincialWebApp1.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "تذكرني")]
        public bool RememberMe { get; set; }
    }
}
