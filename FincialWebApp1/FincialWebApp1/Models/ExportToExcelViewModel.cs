using System.ComponentModel;



namespace FinancialWebApp1.Models
{
    public class ExportToExcelViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FileName { get; set; }
    }

}
