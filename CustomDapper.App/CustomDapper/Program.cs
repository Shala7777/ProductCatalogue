using DatabaseHelper.SqlServer;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace CustomDapper
{

    internal class Program
    {
        static void Main(string[] args)
        {
            using Database database = new Database(
           "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ProductsCatalogue;Integrated Security=True;Encrypt=True");

            try
            {
                database.BeginTransaction();

                database.ExecuteNonQuery("sp_InsertCatalogueItem", commandType: System.Data.CommandType.StoredProcedure,
                         new SqlParameter("@CategoryName", "PC12"),
                         new SqlParameter("@CategoryIsDeleted", 1),
                         new SqlParameter("@ProductCode", 1234512),
                         new SqlParameter("@ProductName", "KeyBoard1"),
                         new SqlParameter("@ProductPrice", 1213123),
                         new SqlParameter("@ProductQuantity", 12),
                         new SqlParameter("@ProductIsDeleted", 1)
                        );

                database.CommitTransaction();
            }
            catch (DbException ex)
            {
                database.RollbackTransaction();
                Console.WriteLine(ex.Message);
            }
            catch (InvalidOperationException opException)
            {
                database.RollbackTransaction();
                Console.WriteLine(opException);
            }
            catch (Exception ex)
            {
                database.RollbackTransaction();
                Console.WriteLine(ex);
            }
        }
    }
}
