using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPRapp.Classes
{
    public class Specialties
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? DepartmentId { get; set; }

        public Specialties(int Id, string Code, string Name, int? DepartmentId = null)
        {
            this.Id = Id;
            this.Code = Code;
            this.Name = Name;
            this.DepartmentId = DepartmentId;
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

        public static List<Specialties> Select()
        {
            List<Specialties> AllSpecialties = new List<Specialties>();
            string SQL = "SELECT * FROM `specialties`;";
            MySqlConnection connection = OpenConnection();
            MySqlDataReader Data = Query(SQL, connection);
            while (Data.Read())
            {
                AllSpecialties.Add(new Specialties(
                    Data.GetInt32(0),
                    Data.GetString(1),
                    Data.GetString(2),
                    Data.IsDBNull(3) ? (int?)null : Data.GetInt32(3)
                ));
            }
            CloseConnection(connection);
            return AllSpecialties;
        }

        public static List<Specialties> SelectByDepartment(int departmentId)
        {
            List<Specialties> result = new List<Specialties>();
            string SQL = "SELECT * FROM `specialties` WHERE DepartmentId = @DeptId;";
            MySqlConnection connection = OpenConnection();
            var cmd = new MySqlCommand(SQL, connection);
            cmd.Parameters.AddWithValue("@DeptId", departmentId);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Specialties(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3)
                ));
            }
            CloseConnection(connection);
            return result;
        }

        public void Add()
        {
            string SQL = "INSERT INTO `specialties`(`Code`, `Name`, `DepartmentId`) VALUES (@Code, @Name, @DepartmentId)";
            MySqlConnection connection = OpenConnection();
            var cmd = new MySqlCommand(SQL, connection);
            cmd.Parameters.AddWithValue("@Code", Code);
            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@DepartmentId", (object)DepartmentId ?? DBNull.Value);
            cmd.ExecuteNonQuery();
            CloseConnection(connection);
        }

        public void Update()
        {
            if (string.IsNullOrWhiteSpace(Code) || string.IsNullOrWhiteSpace(Name))
            {
                throw new Exception("Код и название специальности не могут быть пустыми");
            }

            var existingSpecs = Classes.Specialties.Select();
            if (existingSpecs.Any(s => s.Code.ToLower() == this.Code.ToLower() && s.Id != Id))
            {
                throw new Exception($"Специальность с кодом \"{this.Code}\" уже существует");
            }

            string SQL = "UPDATE `specialties` SET `Code`=@Code, `Name`=@Name, `DepartmentId`=@DepartmentId WHERE `Id`=@Id";
            MySqlConnection connection = OpenConnection();
            var cmd = new MySqlCommand(SQL, connection);
            cmd.Parameters.AddWithValue("@Code", Code);
            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@DepartmentId", (object)DepartmentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.ExecuteNonQuery();
            CloseConnection(connection);
        }

        public void Delete()
        {
            MySqlConnection conn = OpenConnection();
            conn.Open();

            string checkSQL = "SELECT COUNT(*) FROM `groups` WHERE SpecialtyId = @Id";
            var checkCmd = new MySqlCommand(checkSQL, conn);
            checkCmd.Parameters.AddWithValue("@Id", Id);
            int usageCount = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (usageCount > 0)
            {
                CloseConnection(conn);
                throw new Exception($"Невозможно удалить специальность \"{Code} {Name}\": к ней привязано {usageCount} групп.");
            }

            // Удаляем
            string deleteSQL = "DELETE FROM `specialties` WHERE `Id` = @Id";
            var deleteCmd = new MySqlCommand(deleteSQL, conn);
            deleteCmd.Parameters.AddWithValue("@Id", Id);
            deleteCmd.ExecuteNonQuery();

            CloseConnection(conn);
        }
    }
}