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
            this.Id = Id; this.Nomer_paketa = Nomer_paketa; this.Pol = Pol;
            this.Kod = Kod; this.FIO = FIO; this.Otmetka_ov_otsytstvii = Otmetka_ov_otsytstvii;
            this.Sredniy_ball_attestata = Sredniy_ball_attestata;
            this.Otsenka_po_russkomy_yaziky = Otsenka_po_russkomy_yaziky;
            this.Otsenka_po_matematike = Otsenka_po_matematike;
            this.Forma_obychenya = Forma_obychenya;
            this.Group_name = Group_name;
            this.Spisok_BPR_Id = Spisok_BPR_Id;
        }

        public static List<Student_Info> Select()
        {
            List<Student_Info> students = new List<Student_Info>();
            using (MySqlConnection conn = Connection.OpenConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT `Id`, `Nomer_paketa`, `Pol`, `Kod`, `FIO`, `Otmetka_ov_otsytstvii`, `Sredniy_ball_attestata`, `Otsenka_po_russkomy_yaziky`, `Otsenka_po_matematike`, `Forma_obychenya`, `Group_name`, `Spisok_BPR_Id` FROM `student_info`;", conn))
                using (MySqlDataReader data = cmd.ExecuteReader())
                {
                    while (data.Read())
                    {
                        students.Add(new Student_Info(
                            data.GetInt32(0), data.IsDBNull(1) ? "" : data.GetString(1), data.GetString(2), data.IsDBNull(3) ? "" : data.GetString(3),
                            data.GetString(4), data.IsDBNull(5) ? "" : data.GetString(5), data.IsDBNull(6) ? "" : data.GetString(6),
                            data.IsDBNull(7) ? "" : data.GetString(7), data.IsDBNull(8) ? "" : data.GetString(8), data.GetString(9),
                            data.GetInt32(10), data.IsDBNull(11) ? (int?)null : data.GetInt32(11)
                        ));
                    }
                }
            }
            return students;
        }

        public static List<Student_Info> SelectByBPR(int bprId)
        {
            List<Student_Info> students = new List<Student_Info>();
            using (MySqlConnection conn = Connection.OpenConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT `Id`, `Nomer_paketa`, `Pol`, `Kod`, `FIO`, `Otmetka_ov_otsytstvii`, `Sredniy_ball_attestata`, `Otsenka_po_russkomy_yaziky`, `Otsenka_po_matematike`, `Forma_obychenya`, `Group_name`, `Spisok_BPR_Id` FROM `student_info` WHERE Spisok_BPR_Id = @BPRId;", conn))
                {
                    cmd.Parameters.AddWithValue("@BPRId", bprId);
                    using (MySqlDataReader data = cmd.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            students.Add(new Student_Info(
                                data.GetInt32(0), data.IsDBNull(1) ? "" : data.GetString(1), data.GetString(2), data.IsDBNull(3) ? "" : data.GetString(3),
                                data.GetString(4), data.IsDBNull(5) ? "" : data.GetString(5), data.IsDBNull(6) ? "" : data.GetString(6),
                                data.IsDBNull(7) ? "" : data.GetString(7), data.IsDBNull(8) ? "" : data.GetString(8), data.GetString(9),
                                data.GetInt32(10), data.IsDBNull(11) ? (int?)null : data.GetInt32(11)
                            ));
                        }
                    }
                }
            }
            return students;
        }

        // 🔹 Обёртка для старого кода
        public void Add()
        {
            if (!Add(out string error))
                throw new System.Exception(error);
        }

        // 🔹 Новый безопасный метод с out
        public bool Add(out string errorMessage)
        {
            errorMessage = "";
            if (Spisok_BPR_Id.HasValue)
            {
                var existing = Select().FirstOrDefault(s =>
                    s.FIO.Equals(this.FIO, StringComparison.OrdinalIgnoreCase) &&
                    s.Spisok_BPR_Id == this.Spisok_BPR_Id);
                if (existing != null)
                {
                    errorMessage = $"Студент \"{this.FIO}\" уже добавлен в этот список ВПР";
                    return false;
                }
            }

            try
            {
                using (MySqlConnection conn = Connection.OpenConnection())
                {
                    string sql = "INSERT INTO `student_info` " +
                        "(`Nomer_paketa`, `Pol`, `Kod`, `FIO`, `Otmetka_ov_otsytstvii`, `Sredniy_ball_attestata`, " +
                        "`Otsenka_po_russkomy_yaziky`, `Otsenka_po_matematike`, `Forma_obychenya`, `Group_name`, `Spisok_BPR_Id`) " +
                        "VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11);";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@p1", Nomer_paketa ?? "");
                        cmd.Parameters.AddWithValue("@p2", Pol ?? "");
                        cmd.Parameters.AddWithValue("@p3", Kod ?? "");
                        cmd.Parameters.AddWithValue("@p4", FIO ?? "");
                        cmd.Parameters.AddWithValue("@p5", Otmetka_ov_otsytstvii ?? "");
                        cmd.Parameters.AddWithValue("@p6", Sredniy_ball_attestata ?? "");
                        cmd.Parameters.AddWithValue("@p7", Otsenka_po_russkomy_yaziky ?? "");
                        cmd.Parameters.AddWithValue("@p8", Otsenka_po_matematike ?? "");
                        cmd.Parameters.AddWithValue("@p9", Forma_obychenya ?? "");
                        cmd.Parameters.AddWithValue("@p10", Group_name);
                        cmd.Parameters.AddWithValue("@p11", (object)Spisok_BPR_Id ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
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
            using (MySqlConnection conn = Connection.OpenConnection())
            {
                string sql = "UPDATE `student_info` SET " +
                    "`Nomer_paketa`=@p1, `Pol`=@p2, `Kod`=@p3, `FIO`=@p4, `Otmetka_ov_otsytstvii`=@p5, " +
                    "`Sredniy_ball_attestata`=@p6, `Otsenka_po_russkomy_yaziky`=@p7, `Otsenka_po_matematike`=@p8, " +
                    "`Forma_obychenya`=@p9, `Group_name`=@p10, `Spisok_BPR_Id`=@p11 WHERE `Id`=@id;";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@p1", Nomer_paketa ?? "");
                    cmd.Parameters.AddWithValue("@p2", Pol ?? "");
                    cmd.Parameters.AddWithValue("@p3", Kod ?? "");
                    cmd.Parameters.AddWithValue("@p4", FIO ?? "");
                    cmd.Parameters.AddWithValue("@p5", Otmetka_ov_otsytstvii ?? "");
                    cmd.Parameters.AddWithValue("@p6", Sredniy_ball_attestata ?? "");
                    cmd.Parameters.AddWithValue("@p7", Otsenka_po_russkomy_yaziky ?? "");
                    cmd.Parameters.AddWithValue("@p8", Otsenka_po_matematike ?? "");
                    cmd.Parameters.AddWithValue("@p9", Forma_obychenya ?? "");
                    cmd.Parameters.AddWithValue("@p10", Group_name);
                    cmd.Parameters.AddWithValue("@p11", (object)Spisok_BPR_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete()
        {
            using (MySqlConnection conn = Connection.OpenConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand("DELETE FROM `student_info` WHERE `Id` = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int CountByBPR(int bprId)
        {
            using (MySqlConnection conn = Connection.OpenConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM student_info WHERE Spisok_BPR_Id = @BPRId;", conn))
                {
                    cmd.Parameters.AddWithValue("@BPRId", bprId);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }
    }
}