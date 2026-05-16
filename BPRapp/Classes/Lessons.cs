using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPRapp.Classes
{
    public class Lessons
    {
        public int Id { get; set; }
        public string Lesson { get; set; }
        public Lessons(int Id, string Lesson) 
        {
            this.Id = Id;
            this.Lesson = Lesson;
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
        public static List<Lessons> Select()
        {
            List<Lessons> AllLessons = new List<Lessons>();
            string SQL = "SELECT * FROM `lessons`;";
            MySqlConnection connection = OpenConnection();
            MySqlDataReader Data = Query(SQL, connection);
            while (Data.Read())
            {
                AllLessons.Add(new Lessons(
                    Data.GetInt32(0),
                    Data.GetString(1)
                    ));
            }
            CloseConnection(connection);
            return AllLessons;
        }
        public void Add()
        {
            if (string.IsNullOrWhiteSpace(Lesson))
            {
                throw new Exception("Название предмета не может быть пустым");
            }

            var existingLessons = Classes.Lessons.Select();
            if (existingLessons.Any(l => l.Lesson.ToLower() == this.Lesson.ToLower() && l.Id != Id))
            {
                throw new Exception($"Предмет \"{this.Lesson}\" уже существует");
            }

            string SQL = "INSERT INTO `lessons`(`Lesson`) VALUES (@Lesson)";
            MySqlConnection connection = OpenConnection();
            var cmd = new MySqlCommand(SQL, connection);
            cmd.Parameters.AddWithValue("@Lesson", Lesson);
            cmd.ExecuteNonQuery();
            CloseConnection(connection);
        }
        public void Update()
        {
            if (string.IsNullOrWhiteSpace(Lesson))
            {
                throw new Exception("Название предмета не может быть пустым");
            }

            var existingLessons = Classes.Lessons.Select();
            if (existingLessons.Any(l => l.Lesson.ToLower() == this.Lesson.ToLower() && l.Id != Id))
            {
                throw new Exception($"Предмет \"{this.Lesson}\" уже существует");
            }

            string SQL = $"UPDATE `lessons` SET `Lesson`='{Lesson}' WHERE `Id`='{Id}'";
            MySqlConnection connection = OpenConnection();
            Query(SQL, connection);
            CloseConnection(connection);
        }

        public void Delete()
        {
            MySqlConnection conn = OpenConnection();
            conn.Open();

            string checkSQL = "SELECT COUNT(*) FROM bpr_info WHERE Lesson = @Id";
            var checkCmd = new MySqlCommand(checkSQL, conn);
            checkCmd.Parameters.AddWithValue("@Id", Id);
            int usageCount = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (usageCount > 0)
            {
                CloseConnection(conn);
                throw new Exception($"Невозможно удалить предмет \"{Lesson}\": он указан в {usageCount} записях ВПР.");
            }

            string deleteSQL = "DELETE FROM `lessons` WHERE `Id` = @Id";
            var deleteCmd = new MySqlCommand(deleteSQL, conn);
            deleteCmd.Parameters.AddWithValue("@Id", Id);
            deleteCmd.ExecuteNonQuery();

            CloseConnection(conn);
        }
    }
}
