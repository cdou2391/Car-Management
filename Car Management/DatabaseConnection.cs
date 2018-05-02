using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Car_Management
{
    class DatabaseConnection
    {
        public static string connectionStr = @"Data Source=LAPTOP-CEDRIC;Initial Catalog = Contacts; Integrated Security = True";
        public static SqlConnection connection = new SqlConnection(connectionStr);
        public string checkDatabase()
        {
            string answer = null;
            using (var conn = new SqlConnection(DatabaseConnection.connectionStr))
            {
                try
                {
                    conn.Open();
                    answer = "true";
                }
                catch (SqlException sql)
                {
                    answer = "false" + sql.Message;
                }

            }
            return answer;
        }
    }
}
