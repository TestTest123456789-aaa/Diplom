using MySql.Data.MySqlClient;
using System.Data;
using System.Threading.Tasks;

namespace BPRapp.Classes
{
    public static class Connection
    {
        public static string server = "localhost";
        public static string login = "root";
        public static string password = "Asdfg123";
        public static string DataBase = "bpr";
        public static string Port = "";
        // pooling=true переиспользует соединения, убирая лаги при открытии/закрытии
        public static string con => $"server={server};uid={login};pwd={password};database={DataBase};port={Port};pooling=true;";

        public static MySqlConnection OpenConnection()
        {
            var conn = new MySqlConnection(con);
            conn.Open();
            return conn;
        }

        public static void CloseConnection(MySqlConnection connection)
        {
            if (connection?.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
        //public static string server = "127.0.0.1";
        //public static string login = "root";
        //public static string password = "";
        //public static string DataBase = "bpr";
        //public static string Port = "3307";
        //public static string con = $"server={server};uid={login};pwd={password};database={DataBase};port={Port};";

        //port={Port};
    }   
}
