using System.Data.Common;
using System.Data.SqlClient;

namespace DatabaseHelper
{
    public class SqlClientDatabase : Database<SqlConnection, SqlCommand, SqlTransaction, SqlParameter, SqlDataReader, SqlDataAdapter, SqlCommandBuilder>
    {
        public SqlClientDatabase(string connectionString, bool useSingletonConncetion = true) : base(connectionString, useSingletonConncetion)
        {

        }
    }
}
