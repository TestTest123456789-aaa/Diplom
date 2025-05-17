using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace BPRapp.Classes
{
    public class Teachers
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public string Login { get; set; }
        public string Parol { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone_Number { get; set; }
        public Teachers(int Id, string FIO, string Login, string Parol, string Role, string Email, string Phone_Number)
        {
            this.Id = Id;
            this.FIO = FIO;
            this.Login = Login;
            this.Parol = Parol;
            this.Role = Role;
            this.Email = Email;
            this.Phone_Number = Phone_Number;
        }
        public static void CloseConnection(MySqlConnection connection)
        {
            connection.Close();
            MySqlConnection.ClearPool(connection);
        }
        public static MySqlDataReader Query(string SQL, MySqlConnection connection)
        {
            return new MySqlCommand(SQL, connection).ExecuteReader();
        }
        public static MySqlConnection OpenConnection()
        {
            MySqlConnection connection = new MySqlConnection(Connection.con);
            connection.Open();

            return connection;
        }
        public static List<Student_Info> Select()
        {
            List<Student_Info> AllStudent_Info = new List<Student_Info>();
            string SQL = "SELECT * FROM `teachers`;";
            MySqlConnection connection = OpenConnection();
            MySqlDataReader Data = Query(SQL, connection);
            while (Data.Read())
            {
                AllStudent_Info.Add(new Student_Info(
                    Data.GetInt32(0),
                    Data.GetString(1),
                    Data.GetString(2),
                    Data.GetString(3),
                    Data.GetString(4),
                    Data.GetString(5),
                    Data.GetString(6),
                    Data.GetString(7),
                    Data.GetString(8),
                    Data.GetString(9),
                    Data.GetString(10)
                    ));
            }
            CloseConnection(connection);
            return AllStudent_Info;
        }
        public void Add()
        {
            string SQL = $"INSERT INTO `teachers`(`FIO`, `Login`, `Parol`, `Role`, `Email`, `Phone_Number`) " +
                         $"VALUES ('{FIO}','{Login}','{Parol}','{Role}','{Email}','{Phone_Number}')";
            MySqlConnection connection = OpenConnection();
            Query(SQL, connection);
            CloseConnection(connection);
        }
        public void Update()
        {
            string SQL = $"UPDATE `teachers` SET `Nomer_paketa`='{FIO}',`Pol`='{FIO}',`Login`='{Login}',`Parol`='{Parol}',`Role`='{Role}',`Email`='{Email}'," +
                         $"`Phone_Number`='{Phone_Number}' WHERE `Id`='{Id}'";
            MySqlConnection connection = OpenConnection();
            Query(SQL, connection);
            CloseConnection(connection);
        }

        public void Delete()
        {
            string SQL = $"DELETE FROM `teachers` WHERE `Id` = {Id}";
            MySqlConnection connection = OpenConnection();
            Query(SQL, connection);
            CloseConnection(connection);
        }
    }
}