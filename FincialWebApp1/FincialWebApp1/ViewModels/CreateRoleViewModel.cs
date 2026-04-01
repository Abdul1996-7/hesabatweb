using System.ComponentModel.DataAnnotations;

namespace FincialWebApp1.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
