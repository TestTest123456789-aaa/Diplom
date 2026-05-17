using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPRapp.Classes
{
    public class Departments
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? HeadTeacherId { get; set; }

        public Departments(int Id, string Name, int? HeadTeacherId = null)
        {
            this.Id = Id;
            this.Name = Name;
            this.HeadTeacherId = HeadTeacherId;
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

        public static List<Departments> Select()
        {
            List<Departments> AllDepartments = new List<Departments>();
            string SQL = "SELECT * FROM `departments`;";
            MySqlConnection connection = OpenConnection();
            MySqlDataReader Data = Query(SQL, connection);
            while (Data.Read())
            {
                AllDepartments.Add(new Departments(
                    Data.GetInt32(0),
                    Data.GetString(1),
                    Data.IsDBNull(2) ? (int?)null : Data.GetInt32(2)
                ));
            }
            CloseConnection(connection);
            return AllDepartments;
        }

        public void Add()
        {
            // 🔹 ПРОВЕРКА: Название не пустое
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new Exception("Название отделения не может быть пустым");
            }

            // 🔹 ПРОВЕРКА: Уникальность названия (регистронезависимая)
            var existingDepts = Classes.Departments.Select();
            if (existingDepts.Any(d => d.Name.ToLower() == this.Name.ToLower()))
            {
                throw new Exception($"Отделение с названием \"{this.Name}\" уже существует");
            }

            string SQL = "INSERT INTO `departments`(`Name`, `HeadTeacherId`) VALUES (@Name, @HeadTeacherId)";
            MySqlConnection connection = OpenConnection();
            var cmd = new MySqlCommand(SQL, connection);
            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@HeadTeacherId", (object)HeadTeacherId ?? DBNull.Value);
            cmd.ExecuteNonQuery();
            CloseConnection(connection);
        }

        public void Update()
        {
            // 🔹 ПРОВЕРКА: Название не пустое
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new Exception("Название отделения не может быть пустым");
            }

            // 🔹 ПРОВЕРКА: Уникальность названия (исключаем текущую запись)
            var existingDepts = Classes.Departments.Select();
            if (existingDepts.Any(d => d.Name.ToLower() == this.Name.ToLower() && d.Id != Id))
            {
                throw new Exception($"Отделение с названием \"{this.Name}\" уже существует");
            }

            string SQL = "UPDATE `departments` SET `Name`=@Name, `HeadTeacherId`=@HeadTeacherId WHERE `Id`=@Id";
            MySqlConnection connection = OpenConnection();
            var cmd = new MySqlCommand(SQL, connection);
            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@HeadTeacherId", (object)HeadTeacherId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.ExecuteNonQuery();
            CloseConnection(connection);
        }

        public void Delete()
        {
            // ✅ ИСПРАВЛЕНО: убрано двойное открытие соединения
            MySqlConnection conn = OpenConnection();

            try
            {
                // 🔹 Проверка: есть ли группы, привязанные к отделению
                string checkGroupsSQL = "SELECT COUNT(*) FROM `groups` WHERE DepartmentId = @Id";
                var checkGroupsCmd = new MySqlCommand(checkGroupsSQL, conn);
                checkGroupsCmd.Parameters.AddWithValue("@Id", Id);
                int groupCount = Convert.ToInt32(checkGroupsCmd.ExecuteScalar());

                if (groupCount > 0)
                {
                    throw new Exception($"Невозможно удалить отделение \"{Name}\": к нему привязано {groupCount} групп.");
                }

                // 🔹 Проверка: есть ли специальности, привязанные к отделению
                string checkSpecSQL = "SELECT COUNT(*) FROM `specialties` WHERE DepartmentId = @Id";
                var checkSpecCmd = new MySqlCommand(checkSpecSQL, conn);
                checkSpecCmd.Parameters.AddWithValue("@Id", Id);
                int specCount = Convert.ToInt32(checkSpecCmd.ExecuteScalar());

                if (specCount > 0)
                {
                    throw new Exception($"Невозможно удалить отделение \"{Name}\": к нему привязано {specCount} специальностей.");
                }

                // ✅ Всё чисто — удаляем
                string deleteSQL = "DELETE FROM `departments` WHERE `Id` = @Id";
                var deleteCmd = new MySqlCommand(deleteSQL, conn);
                deleteCmd.Parameters.AddWithValue("@Id", Id);
                deleteCmd.ExecuteNonQuery();
            }
            finally
            {
                CloseConnection(conn);
            }
        }

        public string GetHeadTeacherName()
        {
            if (!HeadTeacherId.HasValue) return "Не назначен";
            var teacher = Users.Select().FirstOrDefault(t => t.Id == HeadTeacherId.Value);
            return teacher?.FIO ?? "Не назначен";
        }
    }
}