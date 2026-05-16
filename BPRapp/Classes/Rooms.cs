using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPRapp.Classes
{
    public class Rooms
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }

        // Добавляем свойство для отображения в ComboBox
        public string DisplayName
        {
            get
            {
                return $"{Name} ({Capacity} мест)";
            }
        }

        public Rooms(int Id, string Name, int Capacity)
        {
            this.Id = Id;
            this.Name = Name;
            this.Capacity = Capacity;
        }

        public static void CloseConnection(MySqlConnection connection)
        {
            connection.Close(); MySqlConnection.ClearPool(connection);
        }

        public static MySqlDataReader Query(string SQL, MySqlConnection connection) => new MySqlCommand(SQL, connection).ExecuteReader();
        public static MySqlConnection OpenConnection() => new MySqlConnection(Connection.con)
        {
            ConnectionString = Connection.con
        };

        public static List<Rooms> Select()
        {
            List<Rooms> list = new List<Rooms>();
            MySqlConnection conn = OpenConnection(); conn.Open();
            MySqlDataReader r = Query("SELECT * FROM `rooms`;", conn);
            while (r.Read())
                list.Add(new Rooms(r.GetInt32(0), r.GetString(1), r.GetInt32(2)));
            CloseConnection(conn);
            return list;
        }

        public void Add()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new Exception("Название кабинета не может быть пустым");
            }

            var existingRooms = Classes.Rooms.Select();
            if (existingRooms.Any(r => r.Name.ToLower() == this.Name.ToLower() && r.Id != Id))
            {
                throw new Exception($"Кабинет \"{this.Name}\" уже существует");
            }

            string SQL = "INSERT INTO `rooms`(`Name`, `Capacity`) VALUES (@Name, @Capacity)";
            MySqlConnection conn = OpenConnection(); conn.Open();
            var cmd = new MySqlCommand(SQL, conn);
            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@Capacity", Capacity);
            cmd.ExecuteNonQuery(); CloseConnection(conn);
        }

        public void Update()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new Exception("Название кабинета не может быть пустым");
            }

            var existingRooms = Classes.Rooms.Select();
            if (existingRooms.Any(r => r.Name.ToLower() == this.Name.ToLower() && r.Id != Id))
            {
                throw new Exception($"Кабинет \"{this.Name}\" уже существует");
            }

            string SQL = "UPDATE `rooms` SET `Name`=@Name, `Capacity`=@Capacity WHERE `Id`=@Id";
            MySqlConnection conn = OpenConnection(); conn.Open();
            var cmd = new MySqlCommand(SQL, conn);
            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@Capacity", Capacity);
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.ExecuteNonQuery(); CloseConnection(conn);
        }

        public void Delete()
        {
            MySqlConnection conn = OpenConnection();
            conn.Open();

            string checkSQL = "SELECT COUNT(*) FROM bpr_info WHERE RoomId = @Id";
            var checkCmd = new MySqlCommand(checkSQL, conn);
            checkCmd.Parameters.AddWithValue("@Id", Id);
            int usageCount = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (usageCount > 0)
            {
                CloseConnection(conn);
                throw new Exception($"Невозможно удалить кабинет \"{Name}\": он используется в {usageCount} записях ВПР.");
            }

            string deleteSQL = "DELETE FROM `rooms` WHERE `Id` = @Id";
            var deleteCmd = new MySqlCommand(deleteSQL, conn);
            deleteCmd.Parameters.AddWithValue("@Id", Id);
            deleteCmd.ExecuteNonQuery();

            CloseConnection(conn);
        }
    }
}