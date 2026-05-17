using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPRapp.Classes
{
    public class BPR_info
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public int Count_Students { get; set; }
        public int Responsible_user { get; set; }
        public int Lesson { get; set; }
        public int? GroupId { get; set; }
        public int? RoomId { get; set; }

        public BPR_info(int Id, string Date, string Time, int Count_Students,
            int Responsible_user, int Lesson, int? GroupId = null, int? RoomId = null)
        {
            this.Id = Id;
            this.Date = Date;
            this.Time = Time;
            this.Count_Students = Count_Students;
            this.Responsible_user = Responsible_user;
            this.Lesson = Lesson;
            this.GroupId = GroupId;
            this.RoomId = RoomId;
        }

        public static List<BPR_info> Select()
        {
            List<BPR_info> list = new List<BPR_info>();
            using (MySqlConnection conn = Connection.OpenConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT `Id`, `Date`, `Time`, `Count_Students`, `Responsible_user`, `Lesson`, `GroupId`, `RoomId` FROM `bpr_info`;", conn))
                {
                    using (MySqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new BPR_info(
                                r.GetInt32(0),
                                r.GetString(1),
                                r.GetString(2),
                                r.IsDBNull(3) ? 0 : r.GetInt32(3),
                                r.GetInt32(4),
                                r.GetInt32(5),
                                r.IsDBNull(6) ? (int?)null : r.GetInt32(6),
                                r.IsDBNull(7) ? (int?)null : r.GetInt32(7)
                            ));
                        }
                    }
                }
            }
            return list;
        }

        public void Add()
        {
            using (MySqlConnection conn = Connection.OpenConnection())
            {
                string sql = "INSERT INTO `bpr_info`(`Date`, `Time`, `Count_Students`, `Responsible_user`, `Lesson`, `GroupId`, `RoomId`) VALUES (@Date, @Time, @Count, @Resp, @Lesson, @GroupId, @RoomId)";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Date", Date);
                    cmd.Parameters.AddWithValue("@Time", Time);
                    cmd.Parameters.AddWithValue("@Count", Count_Students);
                    cmd.Parameters.AddWithValue("@Resp", Responsible_user);
                    cmd.Parameters.AddWithValue("@Lesson", Lesson);
                    cmd.Parameters.AddWithValue("@GroupId", (object)GroupId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoomId", (object)RoomId ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update()
        {
            using (MySqlConnection conn = Connection.OpenConnection())
            {
                string sql = "UPDATE `bpr_info` SET `Date`=@Date, `Time`=@Time, `Count_Students`=@Count, `Responsible_user`=@Resp, `Lesson`=@Lesson, `GroupId`=@GroupId, `RoomId`=@RoomId WHERE `Id`=@Id";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Date", Date);
                    cmd.Parameters.AddWithValue("@Time", Time);
                    cmd.Parameters.AddWithValue("@Count", Count_Students);
                    cmd.Parameters.AddWithValue("@Resp", Responsible_user);
                    cmd.Parameters.AddWithValue("@Lesson", Lesson);
                    cmd.Parameters.AddWithValue("@GroupId", (object)GroupId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoomId", (object)RoomId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete()
        {
            using (MySqlConnection conn = Connection.OpenConnection())
            {
                using (MySqlCommand cmdS = new MySqlCommand("DELETE FROM `student_info` WHERE `Spisok_BPR_Id` = @Id", conn))
                {
                    cmdS.Parameters.AddWithValue("@Id", Id);
                    cmdS.ExecuteNonQuery();
                }
                using (MySqlCommand cmdB = new MySqlCommand("DELETE FROM `bpr_info` WHERE `Id` = @Id", conn))
                {
                    cmdB.Parameters.AddWithValue("@Id", Id);
                    cmdB.ExecuteNonQuery();
                }
            }
        }

        public int GetActualStudentCount()
        {
            using (MySqlConnection conn = Connection.OpenConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM student_info WHERE Spisok_BPR_Id = @BPRId;", conn))
                {
                    cmd.Parameters.AddWithValue("@BPRId", Id);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        // 🔹 ИСПРАВЛЕНО: Читаем все данные в список ПЕРЕД выполнением INSERT
        public void UpdateWithStudentSync(int? oldGroupId)
        {
            Update();
            if (oldGroupId != GroupId && GroupId.HasValue)
            {
                // 🔹 Сначала читаем ВСЕ данные в память
                List<string[]> studentsToCopy = new List<string[]>();

                using (MySqlConnection conn = Connection.OpenConnection())
                {
                    using (MySqlCommand selectCmd = new MySqlCommand("SELECT `Pol`, `FIO`, `Forma_obychenya` FROM student_info WHERE Group_name = @GroupId AND Spisok_BPR_Id IS NULL", conn))
                    {
                        selectCmd.Parameters.AddWithValue("@GroupId", GroupId.Value);
                        using (MySqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                studentsToCopy.Add(new string[] {
                                    reader.GetString("Pol"),
                                    reader.GetString("FIO"),
                                    reader.GetString("Forma_obychenya")
                                });
                            }
                            // 🔹 DataReader автоматически закроется здесь
                        }
                    }
                }

                // 🔹 Теперь выполняем DELETE и INSERT с НОВЫМ соединением
                using (MySqlConnection conn = Connection.OpenConnection())
                {
                    // Удаляем старых студентов
                    using (MySqlCommand deleteCmd = new MySqlCommand("DELETE FROM student_info WHERE Spisok_BPR_Id = @BPRId", conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@BPRId", Id);
                        deleteCmd.ExecuteNonQuery();
                    }

                    // Вставляем новых студентов
                    foreach (var s in studentsToCopy)
                    {
                        using (MySqlCommand insertCmd = new MySqlCommand(@"INSERT INTO student_info
                            (Nomer_paketa, Pol, Kod, FIO, Otmetka_ov_otsytstvii, Sredniy_ball_attestata,
                            Otsenka_po_russkomy_yaziky, Otsenka_po_matematike, Forma_obychenya, Group_name, Spisok_BPR_Id)
                            VALUES ('', @Pol, '', @FIO, '', '', '', '', @Forma, @GroupId, @BPRId)", conn))
                        {
                            insertCmd.Parameters.AddWithValue("@Pol", s[0]);
                            insertCmd.Parameters.AddWithValue("@FIO", s[1]);
                            insertCmd.Parameters.AddWithValue("@Forma", s[2]);
                            insertCmd.Parameters.AddWithValue("@GroupId", GroupId.Value);
                            insertCmd.Parameters.AddWithValue("@BPRId", Id);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        // 🔹 Кэшированные геттеры для отображения
        public string GetResponsibleTeacherName() => DataCache.Users.FirstOrDefault(u => u.Id == Responsible_user)?.FIO ?? "Не назначен";
        public string GetLessonName() => DataCache.Lessons.FirstOrDefault(l => l.Id == Lesson)?.Lesson ?? "Не указан";
        public string GetGroupName() => GroupId.HasValue ? DataCache.Groups.FirstOrDefault(g => g.Id == GroupId.Value)?.Name ?? "Не указана" : "Не указана";
        public string GetGroupHeadTeacher()
        {
            if (!GroupId.HasValue) return "—";
            return DataCache.Groups.FirstOrDefault(g => g.Id == GroupId.Value)?.GetHeadTeacherName() ?? "—";
        }
        public string GetRoomName()
        {
            if (!RoomId.HasValue) return "Не указан";
            var room = DataCache.Rooms.FirstOrDefault(r => r.Id == RoomId.Value);
            return room != null ? $"{room.Name} ({room.Capacity} мест)" : "Не указан";
        }
        public string GetGroupSpecialty()
        {
            if (!GroupId.HasValue) return "Не указана";
            var group = DataCache.Groups.FirstOrDefault(g => g.Id == GroupId.Value);
            if (group == null || !group.SpecialtyId.HasValue) return "Не указана";
            var specialty = DataCache.Specialties.FirstOrDefault(s => s.Id == group.SpecialtyId.Value);
            return specialty != null ? $"{specialty.Code} \"{specialty.Name}\"" : "Не указана";
        }

        public string DisplayInfo => $"👥 {GetGroupName()} | 📚 {GetLessonName()}";
    }
}