using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPRapp.Classes
{
    public class Groups
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? HeadTeacherId { get; set; }
        public int? DepartmentId { get; set; }
        public int? SpecialtyId { get; set; }

        public Groups(int Id, string Name, int? HeadTeacherId = null, int? DepartmentId = null, int? SpecialtyId = null)
        {
            this.Id = Id;
            this.Name = Name;
            this.HeadTeacherId = HeadTeacherId;
            this.DepartmentId = DepartmentId;
            this.SpecialtyId = SpecialtyId;
        }

        public static void CloseConnection(MySqlConnection connection) { connection.Close(); MySqlConnection.ClearPool(connection); }
        public static MySqlDataReader Query(string SQL, MySqlConnection connection) => new MySqlCommand(SQL, connection).ExecuteReader();
        public static MySqlConnection OpenConnection() => new MySqlConnection(Connection.con) { ConnectionString = Connection.con };

        public static List<Groups> Select()
        {
            List<Groups> AllGroups = new List<Groups>();
            string SQL = "SELECT * FROM `groups`;";
            MySqlConnection connection = OpenConnection();
            connection.Open();
            MySqlDataReader Data = Query(SQL, connection);
            while (Data.Read())
            {
                AllGroups.Add(new Groups(
                    Data.GetInt32(0),
                    Data.GetString(1),
                    Data.IsDBNull(2) ? (int?)null : Data.GetInt32(2),
                    Data.IsDBNull(3) ? (int?)null : Data.GetInt32(3),
                    Data.IsDBNull(4) ? (int?)null : Data.GetInt32(4)
                ));
            }
            CloseConnection(connection);
            return AllGroups;
        }

        public void Add()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new Exception("Название группы не может быть пустым");
            }

            var existingGroups = Classes.Groups.Select();
            if (existingGroups.Any(g => g.Name.ToLower() == Name.ToLower() && g.Id != Id))
            {
                throw new Exception($"Группа с названием \"{Name}\" уже существует");
            }

            string SQL = "INSERT INTO `groups`(`Name`, `HeadTeacherId`, `DepartmentId`, `SpecialtyId`) VALUES (@Name, @Head, @Dept, @Spec)";
            MySqlConnection connection = OpenConnection(); connection.Open();
            var cmd = new MySqlCommand(SQL, connection);
            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@Head", (object)HeadTeacherId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Dept", (object)DepartmentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Spec", (object)SpecialtyId ?? DBNull.Value);
            cmd.ExecuteNonQuery();
            CloseConnection(connection);
        }

        public void Update()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new Exception("Название группы не может быть пустым");
            }

            var existingGroups = Classes.Groups.Select();
            if (existingGroups.Any(g => g.Name.ToLower() == Name.ToLower() && g.Id != Id))
            {
                throw new Exception($"Группа с названием \"{Name}\" уже существует");
            }

            string SQL = "UPDATE `groups` SET `Name`=@Name, `HeadTeacherId`=@Head, `DepartmentId`=@Dept, `SpecialtyId`=@Spec WHERE `Id`=@Id";
            MySqlConnection connection = OpenConnection(); connection.Open();
            var cmd = new MySqlCommand(SQL, connection);
            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@Head", (object)HeadTeacherId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Dept", (object)DepartmentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Spec", (object)SpecialtyId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.ExecuteNonQuery();
            CloseConnection(connection);
        }

        public void Delete()
        {
            MySqlConnection conn = OpenConnection();
            conn.Open();

            string checkStudentsSQL = "SELECT COUNT(*) FROM student_info WHERE Group_name = @Id";
            var checkStudentsCmd = new MySqlCommand(checkStudentsSQL, conn);
            checkStudentsCmd.Parameters.AddWithValue("@Id", Id);
            int studentCount = Convert.ToInt32(checkStudentsCmd.ExecuteScalar());

            if (studentCount > 0)
            {
                CloseConnection(conn);
                throw new Exception($"Невозможно удалить группу \"{Name}\": в ней числится {studentCount} студентов.");
            }

            string checkBPRSQL = "SELECT COUNT(*) FROM bpr_info WHERE GroupId = @Id";
            var checkBPRCmd = new MySqlCommand(checkBPRSQL, conn);
            checkBPRCmd.Parameters.AddWithValue("@Id", Id);
            int bprCount = Convert.ToInt32(checkBPRCmd.ExecuteScalar());

            if (bprCount > 0)
            {
                CloseConnection(conn);
                throw new Exception($"Невозможно удалить группу \"{Name}\": она указана в {bprCount} записях ВПР.");
            }

            string deleteSQL = "DELETE FROM `groups` WHERE `Id` = @Id";
            var deleteCmd = new MySqlCommand(deleteSQL, conn);
            deleteCmd.Parameters.AddWithValue("@Id", Id);
            deleteCmd.ExecuteNonQuery();

            CloseConnection(conn);
        }

        public string GetHeadTeacherName() => !HeadTeacherId.HasValue ? "Не назначен" : (Users.Select().FirstOrDefault(t => t.Id == HeadTeacherId.Value)?.FIO ?? "Не назначен");
        public string GetDepartmentName() => !DepartmentId.HasValue ? "Не указано" : (Classes.Departments.Select().FirstOrDefault(d => d.Id == DepartmentId.Value)?.Name ?? "Не указано");
        public string GetSpecialtyName()
        {
            if (!SpecialtyId.HasValue) return "Не указана";
            var spec = Specialties.Select().FirstOrDefault(s => s.Id == SpecialtyId.Value);
            return spec != null ? $"{spec.Code} \"{spec.Name}\"" : "Не указана";
        }
    }
}