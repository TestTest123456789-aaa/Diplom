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
        public string EmailNotifications { get; set; } = "1";

        public Users(int Id, string FIO, string Login, string Parol, string Role, string Email, string Phone_Number, string EmailNotifications = "1")
        {
            this.Id = Id;
            this.FIO = FIO;
            this.Login = Login;
            this.Parol = Parol;
            this.Role = Role;
            this.Email = Email;
            this.Phone_Number = Phone_Number;
            this.EmailNotifications = EmailNotifications;
        }

        public static void CloseConnection(MySqlConnection connection)
        {
            connection.Close();
            MySqlConnection.ClearPool(connection);
        }

        public static MySqlDataReader Query(string SQL, MySqlConnection connection) => new MySqlCommand(SQL, connection).ExecuteReader();

        public static MySqlConnection OpenConnection()
        {
            var c = new MySqlConnection(Connection.con);
            c.Open();
            return c;
        }

        public bool ValidateNoDuplicates(out string errorMessage)
        {
            errorMessage = "";
            MySqlConnection conn = null;
            try
            {
                conn = OpenConnection();
                string checkSQL = "SELECT COUNT(*) FROM teachers WHERE Id <> @Id AND ((Login = @Login AND @Login != '') OR (Email IS NOT NULL AND Email <> '' AND Email = @Email) OR (Phone_Number IS NOT NULL AND Phone_Number <> '' AND Phone_Number = @Phone))";
                using (var cmd = new MySqlCommand(checkSQL, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", this.Id);
                    cmd.Parameters.AddWithValue("@Login", this.Login ?? "");
                    cmd.Parameters.AddWithValue("@Email", this.Email ?? "");
                    cmd.Parameters.AddWithValue("@Phone", this.Phone_Number ?? "");
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > 0)
                    {
                        errorMessage = "❌ Такой Логин, Email или Телефон уже используется в системе. Введите уникальные данные.";
                        return false;
                    }
                }
                return true;
            }
            finally
            {
                if (conn != null) CloseConnection(conn);
            }
        }

        public void Add()
        {
            if (Role == "Студент") throw new Exception("Роль 'Студент' больше не поддерживается");

            if (!ValidateNoDuplicates(out string errorMsg))
                throw new Exception(errorMsg);

            MySqlConnection conn = null;
            try
            {
                conn = OpenConnection();
                string sql = "INSERT INTO teachers (FIO, Login, Parol, Role, Email, Phone_Number) VALUES (@FIO, @Login, @Parol, @Role, @Email, @Phone)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FIO", FIO);
                    cmd.Parameters.AddWithValue("@Login", Login);
                    cmd.Parameters.AddWithValue("@Parol", Parol);
                    cmd.Parameters.AddWithValue("@Role", Role);
                    cmd.Parameters.AddWithValue("@Email", (object)Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", (object)Phone_Number ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (conn != null) CloseConnection(conn);
            }
        }

        public void Update()
        {
            if (!ValidateNoDuplicates(out string errorMsg))
                throw new Exception(errorMsg);

            MySqlConnection conn = null;
            try
            {
                conn = OpenConnection();
                string sql = "UPDATE teachers SET FIO=@FIO, Login=@Login, Parol=@Parol, Role=@Role, Email=@Email, Phone_Number=@Phone WHERE Id=@Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FIO", FIO);
                    cmd.Parameters.AddWithValue("@Login", Login);
                    cmd.Parameters.AddWithValue("@Parol", Parol);
                    cmd.Parameters.AddWithValue("@Role", Role);
                    cmd.Parameters.AddWithValue("@Email", (object)Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", (object)Phone_Number ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (conn != null) CloseConnection(conn);
            }
        }

        public string GetDeleteRestrictions()
        {
            MySqlConnection conn = null;
            try
            {
                conn = OpenConnection();
                var restrictions = new List<string>();

                // Проверка ВПР
                string bprSQL = "SELECT COUNT(*), GROUP_CONCAT(Date SEPARATOR ', ') FROM bpr_info WHERE Responsible_user = @Id";
                using (var cmd = new MySqlCommand(bprSQL, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && reader.GetInt32(0) > 0)
                        {
                            string dates = reader.IsDBNull(1) ? "" : reader.GetString(1);
                            restrictions.Add($"• Записи ВПР ({reader.GetInt32(0)} шт.): {dates}");
                        }
                    }
                }

                // Проверка групп
                string groupsSQL = "SELECT COUNT(*), GROUP_CONCAT(Name SEPARATOR ', ') FROM `groups` WHERE HeadTeacherId = @Id";
                using (var cmd = new MySqlCommand(groupsSQL, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && reader.GetInt32(0) > 0)
                        {
                            string names = reader.IsDBNull(1) ? "" : reader.GetString(1);
                            restrictions.Add($"• Группы ({reader.GetInt32(0)} шт.): {names}");
                        }
                    }
                }

                // Проверка отделений
                string deptsSQL = "SELECT COUNT(*), GROUP_CONCAT(Name SEPARATOR ', ') FROM departments WHERE HeadTeacherId = @Id";
                using (var cmd = new MySqlCommand(deptsSQL, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && reader.GetInt32(0) > 0)
                        {
                            string names = reader.IsDBNull(1) ? "" : reader.GetString(1);
                            restrictions.Add($"• Отделения ({reader.GetInt32(0)} шт.): {names}");
                        }
                    }
                }

                return restrictions.Count > 0 ? string.Join("\n", restrictions) : null;
            }
            finally
            {
                if (conn != null) CloseConnection(conn);
            }
        }

        public void Delete()
        {
            if (Role == "Администратор")
            {
                var allAdmins = Select().Where(u => u.Role == "Администратор").ToList();
                if (allAdmins.Count <= 1) throw new Exception("❌ Нельзя удалить последнего администратора!");
            }

            string restrictions = GetDeleteRestrictions();
            if (!string.IsNullOrEmpty(restrictions))
            {
                throw new Exception($"⚠️ Нельзя удалить преподавателя \"{FIO}\", так как он используется в:\n\n{restrictions}\n\nСначала удалите или переназначьте связанные записи.");
            }

            MySqlConnection conn = null;
            try
            {
                conn = OpenConnection();
                string deleteSQL = "DELETE FROM teachers WHERE Id = @Id";
                using (var cmd = new MySqlCommand(deleteSQL, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (conn != null) CloseConnection(conn);
            }
        }

        public static List<Users> Select()
        {
            List<Users> AllTeachers = new List<Users>();
            string SQL = "SELECT * FROM teachers;";
            MySqlConnection connection = null;
            try
            {
                connection = OpenConnection();
                using (var Data = Query(SQL, connection))
                {
                    while (Data.Read())
                    {
                        AllTeachers.Add(new Users(
                            Data.GetInt32(0),
                            Data.GetString(1),
                            Data.GetString(2),
                            Data.GetString(3),
                            Data.GetString(4),
                            Data.IsDBNull(5) ? "" : Data.GetString(5),
                            Data.IsDBNull(6) ? "" : Data.GetString(6),
                            Data.IsDBNull(7) ? "1" : Data.GetString(7)
                        ));
                    }
                }
                return AllTeachers;
            }
            finally
            {
                if (connection != null) CloseConnection(connection);
            }
        }
    }
}