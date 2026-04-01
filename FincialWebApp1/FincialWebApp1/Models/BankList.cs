using System.ComponentModel.DataAnnotations;

namespace FinancialWebApp1.Models
{
    public class BankList
    {
        [Key]
        public int BankListId { get; set; }
        [Display(Name = " البنك")]
        public string? Name { get; set; }

    }
}
