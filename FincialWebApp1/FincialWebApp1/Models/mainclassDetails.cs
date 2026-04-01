using FinancialWebApp1.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.OleDb;

namespace FincialWebApp1.Models
{
    public class mainclassDetails : ImainClass
    {
        private IConfiguration _configuration;
        private IWebHostEnvironment _environment;
        public mainclassDetails(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }
        public DataTable CustomerDataTable(string path)
        {
            var constr = _configuration.GetConnectionString("excelconnection");
            DataTable datatable = new DataTable();

            constr = string.Format(constr, path);

            using (OleDbConnection excelconn = new OleDbConnection(constr))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    using (OleDbDataAdapter adapterexcel = new OleDbDataAdapter())
                    {

                        excelconn.Open();
                        cmd.Connection = excelconn;
                        DataTable excelschema;
                        excelschema = excelconn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        var sheetname = excelschema.Rows[0]["Table_Name"].ToString();
                        excelconn.Close();

                        excelconn.Open();
                        cmd.CommandText = "SELECT * From [" + sheetname + "]";
                        adapterexcel.SelectCommand = cmd;
                        adapterexcel.Fill(datatable);
                        excelconn.Close();

                    }
                }

            }

            return datatable;
        }

        public string Documentupload(IFormFile fromFiles)
        {
            string uploadpath = _environment.WebRootPath;
            string dest_path = Path.Combine(uploadpath, "uploaded_doc");

            if (!Directory.Exists(dest_path))
            {
                Directory.CreateDirectory(dest_path);
            }
            string sourcefile = Path.GetFileName(fromFiles.FileName);
            string path = Path.Combine(dest_path, sourcefile);

            using (FileStream filestream = new FileStream(path, FileMode.Create))
            {
                fromFiles.CopyTo(filestream);
            }
            return path;
        }

        public void ImportCustomer(DataTable mainclass)
        {
            var sqlconn = _configuration.GetConnectionString("FincialWebApp1Context");

            if (_configuration == null || string.IsNullOrEmpty(sqlconn))
            {
                // Handle the case when _configuration is not initialized or connection string is empty
                return;
            }

            if (mainclass == null || mainclass.Rows.Count == 0)
            {
                // Handle the case when mainclass is null or empty (no data to process)
                return;
            }

            using (SqlConnection scon = new SqlConnection(sqlconn))
            {
                scon.Open();

                // Step 1: Create a temporary staging table
                using (SqlCommand createTempTableCommand = new SqlCommand())
                {
                    createTempTableCommand.Connection = scon;
                    createTempTableCommand.CommandType = CommandType.Text;
                    createTempTableCommand.CommandText = @"
                CREATE TABLE #TempMainClass (
                    CTnumber NVARCHAR(255), -- Adjust the length (e.g., NVARCHAR(255)) as needed,
                    Amount DECIMAL(18, 2),
                    CTdate DATE,
                    IsDeleted BIT NULL
                );
            ";

                    createTempTableCommand.ExecuteNonQuery();
                }

                // Step 2: Bulk Insert new rows into the temporary staging table using SqlBulkCopy
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(scon))
                {
                    sqlBulkCopy.DestinationTableName = "#TempMainClass"; // Name of the temporary staging table

                    // Column mappings (adjust according to your DataTable column names)
                    sqlBulkCopy.ColumnMappings.Add("CTnumber", "CTnumber");
                    sqlBulkCopy.ColumnMappings.Add("Amount", "Amount");
                    sqlBulkCopy.ColumnMappings.Add("CTdate", "CTdate");
                    sqlBulkCopy.ColumnMappings.Add("IsDeleted", "IsDeleted");

                    // Write the DataTable to the temporary staging table using SqlBulkCopy
                    sqlBulkCopy.WriteToServer(mainclass);
                }

                // Step 3: Perform the MERGE operation to insert new rows and update existing rows
                using (SqlCommand mergeCommand = new SqlCommand())
                {
                    mergeCommand.Connection = scon;
                    mergeCommand.CommandType = CommandType.Text;
                    mergeCommand.CommandText = @"
                MERGE INTO MainClass AS target
                USING #TempMainClass AS source
                ON (target.CTnumber = source.CTnumber)
                WHEN MATCHED THEN
                    UPDATE SET
                        target.Amount = source.Amount,
                        target.CTdate = source.CTdate,
                        target.IsDeleted = source.IsDeleted
                WHEN NOT MATCHED THEN
                    INSERT (CTnumber, Amount, CTdate, IsDeleted)
                    VALUES (source.CTnumber, source.Amount, source.CTdate, source.IsDeleted);
            ";

                    mergeCommand.ExecuteNonQuery();
                }

                // Step 4: Drop the temporary staging table
                using (SqlCommand dropTempTableCommand = new SqlCommand())
                {
                    dropTempTableCommand.Connection = scon;
                    dropTempTableCommand.CommandType = CommandType.Text;
                    dropTempTableCommand.CommandText = "DROP TABLE #TempMainClass;";
                    dropTempTableCommand.ExecuteNonQuery();
                }

                // The connection will be automatically closed when exiting the using block
            }
        }
    }
  }
