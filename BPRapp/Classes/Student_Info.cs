using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPRapp.Classes
{
    public class Student_Info
    {
        public int Id { get; set; }
        public string Nomer_paketa { get; set; }
        public string Pol { get; set; }
        public string Kod { get; set; }
        public string FIO { get; set; }
        public string Otmetka_ov_otsytstvii { get; set; }
        public string Sredniy_ball_attestata { get; set; }
        public string Otsenka_po_russkomy_yaziky { get; set; }
        public string Otsenka_po_matematike { get; set; }
        public string Forma_obychenya { get; set; }
        public int Group_name { get; set; }
        public int? Spisok_BPR_Id { get; set; }
        public Student_Info(int Id, string Nomer_paketa, string Pol, string Kod, string FIO,
                    string Otmetka_ov_otsytstvii, string Sredniy_ball_attestata,
                    string Otsenka_po_russkomy_yaziky, string Otsenka_po_matematike,
                    string Forma_obychenya, int Group_name, int? Spisok_BPR_Id)
        {
            this.Id = Id;
            this.Nomer_paketa = Nomer_paketa;
            this.Pol = Pol;
            this.Kod = Kod;
            this.FIO = FIO;
            this.Otmetka_ov_otsytstvii = Otmetka_ov_otsytstvii;
            this.Sredniy_ball_attestata = Sredniy_ball_attestata;
            this.Otsenka_po_russkomy_yaziky = Otsenka_po_russkomy_yaziky;
            this.Otsenka_po_matematike = Otsenka_po_matematike;
            this.Forma_obychenya = Forma_obychenya;
            this.Group_name = Group_name;
            this.Spisok_BPR_Id = Spisok_BPR_Id;
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
            List<Student_Info> students = new List<Student_Info>();
            string SQL = $"SELECT * FROM student_info";
            MySqlConnection connection = OpenConnection();
            MySqlDataReader data = Query(SQL, connection);
            while (data.Read())
            {
                students.Add(new Student_Info(
                    data.GetInt32(0),
                    data.GetString(1),
                    data.GetString(2),
                    data.GetString(3),
                    data.GetString(4),
                    data.GetString(5),
                    data.GetString(6),
                    data.GetString(7),
                    data.GetString(8),
                    data.GetString(9),
                    data.GetInt32(10),
                    data.IsDBNull(11) ? null : (int?)data.GetInt32(11)
                ));
            }
            CloseConnection(connection);
            return students;
        }

        public static List<Student_Info> SelectByBPR(int bprId)
        {
            List<Student_Info> students = new List<Student_Info>();
            string SQL = $"SELECT * FROM student_info WHERE Spisok_BPR_Id = {bprId}";
            MySqlConnection connection = OpenConnection();
            MySqlDataReader data = Query(SQL, connection);
            while (data.Read())
            {
                students.Add(new Student_Info(
                    data.GetInt32(0),
                    data.GetString(1),
                    data.GetString(2),
                    data.GetString(3),
                    data.GetString(4),
                    data.GetString(5),
                    data.GetString(6),
                    data.GetString(7),
                    data.GetString(8),
                    data.GetString(9),
                    data.GetInt32(10),
                    data.IsDBNull(11) ? null : (int?)data.GetInt32(11)
                ));
            }
            CloseConnection(connection);
            return students;
        }

        public bool Add(out string errorMessage)
        {
            errorMessage = "";

            if (Spisok_BPR_Id.HasValue)
            {
                var existing = Select().FirstOrDefault(s =>
                    s.FIO == this.FIO &&
                    s.Spisok_BPR_Id == this.Spisok_BPR_Id);

                if (existing != null)
                {
                    errorMessage = $"Студент \"{this.FIO}\" уже добавлен в этот список ВПР";
                    return false;
                }
            }

            try
            {
                string SQL = $"INSERT INTO `student_info` " +
                             $"(`Nomer_paketa`, `Pol`, `Kod`, `FIO`, `Otmetka_ov_otsytstvii`, `Sredniy_ball_attestata`, " +
                             $"`Otsenka_po_russkomy_yaziky`, `Otsenka_po_matematike`, `Forma_obychenya`, `Group_name`, `Spisok_BPR_Id`) " +
                             $"VALUES " +
                             $"('{Nomer_paketa}', '{Pol}', '{Kod}', '{FIO}', '{Otmetka_ov_otsytstvii}', '{Sredniy_ball_attestata}', " +
                             $"'{Otsenka_po_russkomy_yaziky}', '{Otsenka_po_matematike}', '{Forma_obychenya}', '{Group_name}', " +
                             $"{(Spisok_BPR_Id.HasValue ? Spisok_BPR_Id.Value.ToString() : "NULL")});";

                MySqlConnection connection = OpenConnection();
                Query(SQL, connection);
                CloseConnection(connection);

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Ошибка при добавлении: {ex.Message}";
                return false;
            }
        }
        public void Update()
        {
            string SQL = $"UPDATE `student_info` SET " +
                         $"`Nomer_paketa`='{Nomer_paketa}', " +
                         $"`Pol`='{Pol}', " +
                         $"`Kod`='{Kod}', " +
                         $"`FIO`='{FIO}', " +
                         $"`Otmetka_ov_otsytstvii`='{Otmetka_ov_otsytstvii}', " +
                         $"`Sredniy_ball_attestata`='{Sredniy_ball_attestata}', " +
                         $"`Otsenka_po_russkomy_yaziky`='{Otsenka_po_russkomy_yaziky}', " +
                         $"`Otsenka_po_matematike`='{Otsenka_po_matematike}', " +
                         $"`Forma_obychenya`='{Forma_obychenya}', " +
                         $"`Group_name`='{Group_name}', " +
                         $"`Spisok_BPR_Id`={(Spisok_BPR_Id.HasValue ? Spisok_BPR_Id.Value.ToString() : "NULL")} " +
                         $"WHERE `Id`={Id}";

            MySqlConnection connection = OpenConnection();
            Query(SQL, connection);
            CloseConnection(connection);
        }

        public void Delete()
        {
            string SQL = $"DELETE FROM `student_info` WHERE `Id` = {Id}";
            MySqlConnection connection = OpenConnection();
            Query(SQL, connection);
            CloseConnection(connection);
        }

        public static int CountByBPR(int bprId)
        {
            string SQL = "SELECT COUNT(*) FROM student_info WHERE Spisok_BPR_Id = @BPRId;";
            MySqlConnection connection = OpenConnection();
            MySqlCommand command = new MySqlCommand(SQL, connection);
            command.Parameters.AddWithValue("@BPRId", bprId);
            object result = command.ExecuteScalar();
            CloseConnection(connection);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // 🔁 ПЕРЕГРУЗКА ДЛЯ ОБРАТНОЙ СОВМЕСТИМОСТИ
        // Позволяет старому коду работать без изменений
        public void Add()
        {
            if (Add(out string errorMessage))
            {
                // Успех — ничего не делаем
            }
            else
            {
                // Если ошибка — выбрасываем исключение как раньше
                throw new System.Exception(errorMessage);
            }
        }
    }
}