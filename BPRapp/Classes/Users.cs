using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPRapp.Classes
{
    public class Users
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public string Login { get; set; }
        public string Parol { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone_Number { get; set; }
        public Users(int Id, string FIO, string Login, string Parol, string Role, string Email, string Phone_Number)
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
        public static List<Users> Select()
        {
            List<Users> AllTeachers = new List<Users>();
            string SQL = "SELECT * FROM `teachers`;";
            MySqlConnection connection = OpenConnection();
            MySqlDataReader Data = Query(SQL, connection);
            while (Data.Read())
            {
                AllTeachers.Add(new Users(
                    Data.GetInt32(0),
                    Data.GetString(1),
                    Data.GetString(2),
                    Data.GetString(3),
                    Data.GetString(4),
                    Data.GetString(5),
                    Data.GetString(6)
                    ));
            }
            CloseConnection(connection);
            return AllTeachers;
        }
        public void Add()
        {
            if (Role == "Студент")
            {
                throw new System.Exception("Роль 'Студент' больше не поддерживается");
            }

            string SQL = $"INSERT INTO `teachers`(`FIO`, `Login`, `Parol`, `Role`, `Email`, `Phone_Number`) " +
                        $"VALUES ('{FIO}','{Login}','{Parol}','{Role}','{Email}','{Phone_Number}')";
            MySqlConnection connection = OpenConnection();
            Query(SQL, connection);
            CloseConnection(connection);
        }
        public void Update()
        {
            string SQL = $"UPDATE `teachers` SET `FIO`='{FIO}',`Login`='{Login}',`Parol`='{Parol}',`Role`='{Role}',`Email`='{Email}'," +
                         $"`Phone_Number`='{Phone_Number}' WHERE `Id`='{Id}'";
            MySqlConnection connection = OpenConnection();
            Query(SQL, connection);
            CloseConnection(connection);
        }

        public void Delete()
        {
            if (Role == "Администратор")
            {
                var allAdmins = Select().Where(u => u.Role == "Администратор").ToList();
                if (allAdmins.Count <= 1)
                {
                    throw new Exception("❌ Нельзя удалить последнего администратора в системе! Сначала создайте нового.");
                }
            }

            MySqlConnection conn = OpenConnection();

            string checkBPRSQL = "SELECT COUNT(*) FROM bpr_info WHERE Responsible_user = @Id";
            var checkBPRCmd = new MySqlCommand(checkBPRSQL, conn);
            checkBPRCmd.Parameters.AddWithValue("@Id", Id);
            int bprCount = Convert.ToInt32(checkBPRCmd.ExecuteScalar());
            if (bprCount > 0)
            {
                CloseConnection(conn);
                throw new Exception($"Невозможно удалить \"{FIO}\": он ответственен за {bprCount} ВПР.");
            }

            string checkGroupsSQL = "SELECT COUNT(*) FROM `groups` WHERE HeadTeacherId = @Id";
            var checkGroupsCmd = new MySqlCommand(checkGroupsSQL, conn);
            checkGroupsCmd.Parameters.AddWithValue("@Id", Id);
            int groupCount = Convert.ToInt32(checkGroupsCmd.ExecuteScalar());
            if (groupCount > 0)
            {
                CloseConnection(conn);
                throw new Exception($"Невозможно удалить \"{FIO}\": он является руководителем {groupCount} групп.");
            }

            string checkDeptsSQL = "SELECT COUNT(*) FROM departments WHERE HeadTeacherId = @Id";
            var checkDeptsCmd = new MySqlCommand(checkDeptsSQL, conn);
            checkDeptsCmd.Parameters.AddWithValue("@Id", Id);
            int deptCount = Convert.ToInt32(checkDeptsCmd.ExecuteScalar());
            if (deptCount > 0)
            {
                CloseConnection(conn);
                throw new Exception($"Невозможно удалить \"{FIO}\": он заведует {deptCount} отделениями.");
            }

            string deleteSQL = "DELETE FROM `teachers` WHERE `Id` = @Id";
            var deleteCmd = new MySqlCommand(deleteSQL, conn);
            deleteCmd.Parameters.AddWithValue("@Id", Id);
            deleteCmd.ExecuteNonQuery();
            CloseConnection(conn);
        }
    }
}