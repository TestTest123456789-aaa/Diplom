using MySql.Data.MySqlClient;

namespace BPRapp.Classes
{
    public class Connection
    {
        public static string server = "localhost";
        public static string login = "root";
        public static string password = "Asdfg123";
        public static string DataBase = "bpr";
        public static string Port = "";
        public static string con = $"server={server};uid={login};pwd={password};database={DataBase};port=;";






        //public static string server = "127.0.0.1";
        //public static string login = "root";
        //public static string password = "";
        //public static string DataBase = "bpr";
        //public static string Port = "3307";
        //public static string con = $"server={server};uid={login};pwd={password};database={DataBase};port={Port};";

        //port={Port};

        public static MySqlDataReader Connect(string query)
        {
            MySqlConnection connection = new MySqlConnection(con);
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            return cmd.ExecuteReader();
        }
        public static MySqlConnection OpenConnection()
        {
            MySqlConnection connection = new MySqlConnection(con);
            connection.Open();
            return connection;
        }

        public static void CloseConnection(MySqlConnection connection)
        {
            if (connection?.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
                MySqlConnection.ClearPool(connection);
            }
        }
    }
}
