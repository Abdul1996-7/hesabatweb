using System.Data;

namespace FincialWebApp1.Models
{
    public interface ImainClass
    {
        string Documentupload(IFormFile formFile);
        DataTable CustomerDataTable(string path);
        void ImportCustomer(DataTable mainclass);
    }
}
