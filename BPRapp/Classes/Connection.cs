using MySql.Data.MySqlClient;

namespace BPRapp.Classes
{
    public class Connection
    {
        public static string server = "localhost";
        public static string login = "root";
        public static string password = "Asdfg123";
        public static string DataBase = "kursovoi";
        //public static string Port = "3306";
        public static string con = $"server={server};uid={login};pwd={password};database={DataBase};";

        //port={Port};

        public static MySqlDataReader Connect(string query)
        {
            MySqlConnection connection = new MySqlConnection(con);
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            return cmd.ExecuteReader();
        }
    }
}
