using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

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
            this.Count_Students = Student_Info.CountByBPR(Id);
            this.Responsible_user = Responsible_user;
            this.Lesson = Lesson;
            this.GroupId = GroupId;
            this.RoomId = RoomId;
        }

        public static void CloseConnection(MySqlConnection connection) { connection.Close(); MySqlConnection.ClearPool(connection); }
        public static MySqlDataReader Query(string SQL, MySqlConnection connection) => new MySqlCommand(SQL, connection).ExecuteReader();
        public static MySqlConnection OpenConnection() => new MySqlConnection(Connection.con) { ConnectionString = Connection.con };

        public static List<BPR_info> Select()
        {
            List<BPR_info> list = new List<BPR_info>();
            MySqlConnection conn = OpenConnection(); conn.Open();
            MySqlDataReader r = Query("SELECT * FROM `bpr_info`;", conn);
            while (r.Read())
                list.Add(new BPR_info(
                    r.GetInt32(0), 
                    r.GetString(1), 
                    r.GetString(2),
                    r.GetInt32(3), 
                    r.GetInt32(4), 
                    r.GetInt32(5),
                    r.IsDBNull(6) ? (int?)null : r.GetInt32(6),
                    r.IsDBNull(7) ? (int?)null : r.GetInt32(7)
                ));
            CloseConnection(conn);
            return list;
        }

        public void Add()
        {
            string SQL = "INSERT INTO `bpr_info`(`Date`, `Time`, `Count_Students`, `Responsible_user`, `Lesson`, `GroupId`, `RoomId`) VALUES (@Date, @Time, @Count, @Resp, @Lesson, @GroupId, @RoomId)";
            MySqlConnection conn = OpenConnection(); conn.Open();
            var cmd = new MySqlCommand(SQL, conn);
            cmd.Parameters.AddWithValue("@Date", Date); cmd.Parameters.AddWithValue("@Time", Time);
            cmd.Parameters.AddWithValue("@Count", Count_Students); cmd.Parameters.AddWithValue("@Resp", Responsible_user);
            cmd.Parameters.AddWithValue("@Lesson", Lesson); cmd.Parameters.AddWithValue("@GroupId", (object)GroupId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RoomId", (object)RoomId ?? DBNull.Value);
            cmd.ExecuteNonQuery(); CloseConnection(conn);
        }

        public void Update()
        {
            string SQL = "UPDATE `bpr_info` SET `Date`=@Date, `Time`=@Time, `Count_Students`=@Count, `Responsible_user`=@Resp, `Lesson`=@Lesson, `GroupId`=@GroupId, `RoomId`=@RoomId WHERE `Id`=@Id";
            MySqlConnection conn = OpenConnection(); conn.Open();
            var cmd = new MySqlCommand(SQL, conn);
            cmd.Parameters.AddWithValue("@Date", Date); cmd.Parameters.AddWithValue("@Time", Time);
            cmd.Parameters.AddWithValue("@Count", Count_Students); cmd.Parameters.AddWithValue("@Resp", Responsible_user);
            cmd.Parameters.AddWithValue("@Lesson", Lesson); cmd.Parameters.AddWithValue("@GroupId", (object)GroupId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RoomId", (object)RoomId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.ExecuteNonQuery(); CloseConnection(conn);
        }

        public void Delete()
        {
            MySqlConnection conn = OpenConnection();
            conn.Open();

            string deleteStudentsSQL = "DELETE FROM `student_info` WHERE `Spisok_BPR_Id` = @Id";
            var cmdStudents = new MySqlCommand(deleteStudentsSQL, conn);
            cmdStudents.Parameters.AddWithValue("@Id", Id);
            cmdStudents.ExecuteNonQuery();

            string deleteBPRSQL = "DELETE FROM `bpr_info` WHERE `Id` = @Id";
            var cmdBPR = new MySqlCommand(deleteBPRSQL, conn);
            cmdBPR.Parameters.AddWithValue("@Id", Id);
            cmdBPR.ExecuteNonQuery();

            CloseConnection(conn);
        }

        public string GetResponsibleTeacherName() => Users.Select().FirstOrDefault(u => u.Id == Responsible_user)?.FIO ?? "Не назначен";
        public string GetLessonName() => Lessons.Select().FirstOrDefault(l => l.Id == Lesson)?.Lesson ?? "Не указан";
        public string GetGroupName() => GroupId.HasValue ? Groups.Select().FirstOrDefault(g => g.Id == GroupId.Value)?.Name ?? "Не указана" : "Не указана";
        public string GetGroupHeadTeacher()
        {
            if (!GroupId.HasValue) return "—";
            var group = Groups.Select().FirstOrDefault(g => g.Id == GroupId.Value);
            return group?.GetHeadTeacherName() ?? "—";
        }

        public string GetRoomName()
        {
            if (!RoomId.HasValue) return "Не указан";
            var room = Rooms.Select().FirstOrDefault(r => r.Id == RoomId.Value);
            return room != null ? $"{room.Name} ({room.Capacity} мест)" : "Не указан";
        }

        public string GetGroupSpecialty()
        {
            if (!GroupId.HasValue) return "Не указана";
            var group = Groups.Select().FirstOrDefault(g => g.Id == GroupId.Value);
            if (group == null || !group.SpecialtyId.HasValue) return "Не указана";
            var specialty = Classes.Specialties.Select().FirstOrDefault(s => s.Id == group.SpecialtyId.Value);
            return specialty != null ? $"{specialty.Code} \"{specialty.Name}\"" : "Не указана";
        }

        public int GetActualStudentCount()
        {
            string SQL = "SELECT COUNT(*) FROM student_info WHERE Spisok_BPR_Id = @BPRId;";
            MySqlConnection conn = Connection.OpenConnection();
            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            cmd.Parameters.AddWithValue("@BPRId", Id);
            object result = cmd.ExecuteScalar();
            Connection.CloseConnection(conn);
            return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
        }

        public void CopyStudentsFromGroup()
        {
            if (!GroupId.HasValue) return;

            MySqlConnection conn = Connection.OpenConnection();
            conn.Open();

            string selectSQL = "SELECT * FROM student_info WHERE Group_name = @GroupId AND Spisok_BPR_Id IS NULL";
            var selectCmd = new MySqlCommand(selectSQL, conn);
            selectCmd.Parameters.AddWithValue("@GroupId", GroupId.Value);

            var reader = selectCmd.ExecuteReader();
            var studentsToCopy = new List<dynamic>();

            while (reader.Read())
            {
                studentsToCopy.Add(new
                {
                    Pol = reader.GetString("Pol"),
                    FIO = reader.GetString("FIO"),
                    Forma_obychenya = reader.GetString("Forma_obychenya")
                });
            }
            reader.Close();
            foreach (var student in studentsToCopy)
            {
                string insertSQL = @"INSERT INTO student_info 
            (Nomer_paketa, Pol, Kod, FIO, Otmetka_ov_otsytstvii, Sredniy_ball_attestata, 
             Otsenka_po_russkomy_yaziky, Otsenka_po_matematike, Forma_obychenya, Group_name, Spisok_BPR_Id) 
            VALUES ('', @Pol, '', @FIO, '', '', '', '', @Forma, @GroupId, @BPRId)";

                var insertCmd = new MySqlCommand(insertSQL, conn);
                insertCmd.Parameters.AddWithValue("@Pol", student.Pol);
                insertCmd.Parameters.AddWithValue("@FIO", student.FIO);
                insertCmd.Parameters.AddWithValue("@Forma", student.Forma_obychenya);
                insertCmd.Parameters.AddWithValue("@GroupId", GroupId.Value);
                insertCmd.Parameters.AddWithValue("@BPRId", Id);
                insertCmd.ExecuteNonQuery();
            }

            Connection.CloseConnection(conn);
        }

        public void UpdateWithGroupChange(int? newGroupId)
        {
            MySqlConnection conn = Connection.OpenConnection();
            conn.Open();

            try
            {
                if (newGroupId == this.GroupId)
                {
                    this.Update();
                    return;
                }
                string deleteOldSQL = "DELETE FROM student_info WHERE Spisok_BPR_Id = @BPRId";
                var deleteCmd = new MySqlCommand(deleteOldSQL, conn);
                deleteCmd.Parameters.AddWithValue("@BPRId", this.Id);
                deleteCmd.ExecuteNonQuery();

                this.GroupId = newGroupId;
                this.Update();

                if (newGroupId.HasValue)
                {
                    string selectSQL = "SELECT * FROM student_info WHERE Group_name = @GroupId AND Spisok_BPR_Id IS NULL";
                    var selectCmd = new MySqlCommand(selectSQL, conn);
                    selectCmd.Parameters.AddWithValue("@GroupId", newGroupId.Value);

                    var reader = selectCmd.ExecuteReader();
                    var studentsToCopy = new List<dynamic>();

                    while (reader.Read())
                    {
                        studentsToCopy.Add(new
                        {
                            Pol = reader.GetString("Pol"),
                            FIO = reader.GetString("FIO"),
                            Forma_obychenya = reader.GetString("Forma_obychenya")
                        });
                    }
                    reader.Close();

                    foreach (var student in studentsToCopy)
                    {
                        string insertSQL = @"INSERT INTO student_info 
                    (Nomer_paketa, Pol, Kod, FIO, Otmetka_ov_otsytstvii, Sredniy_ball_attestata, 
                     Otsenka_po_russkomy_yaziky, Otsenka_po_matematike, Forma_obychenya, Group_name, Spisok_BPR_Id) 
                    VALUES ('', @Pol, '', @FIO, '', '', '', '', @Forma, @GroupId, @BPRId)";

                        var insertCmd = new MySqlCommand(insertSQL, conn);
                        insertCmd.Parameters.AddWithValue("@Pol", student.Pol);
                        insertCmd.Parameters.AddWithValue("@FIO", student.FIO);
                        insertCmd.Parameters.AddWithValue("@Forma", student.Forma_obychenya);
                        insertCmd.Parameters.AddWithValue("@GroupId", newGroupId.Value);
                        insertCmd.Parameters.AddWithValue("@BPRId", this.Id);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
            finally
            {
                Connection.CloseConnection(conn);
            }
        }

        public void UpdateWithStudentSync(int? oldGroupId)
        {
            Update();

            if (oldGroupId != GroupId && GroupId.HasValue)
            {
                SyncStudentsWithGroup(oldGroupId);
            }
        }

        private void SyncStudentsWithGroup(int? oldGroupId)
        {
            MySqlConnection conn = Connection.OpenConnection();            

            try
            {
                string deleteSQL = "DELETE FROM student_info WHERE Spisok_BPR_Id = @BPRId";
                var deleteCmd = new MySqlCommand(deleteSQL, conn);
                deleteCmd.Parameters.AddWithValue("@BPRId", Id);
                deleteCmd.ExecuteNonQuery();

                string selectSQL = "SELECT * FROM student_info WHERE Group_name = @NewGroupId AND Spisok_BPR_Id IS NULL";
                var selectCmd = new MySqlCommand(selectSQL, conn);
                selectCmd.Parameters.AddWithValue("@NewGroupId", GroupId.Value);

                var reader = selectCmd.ExecuteReader();
                var studentsToCopy = new List<dynamic>();

                while (reader.Read())
                {
                    studentsToCopy.Add(new
                    {
                        Pol = reader.GetString("Pol"),
                        FIO = reader.GetString("FIO"),
                        Forma_obychenya = reader.GetString("Forma_obychenya")
                    });
                }
                reader.Close();

                foreach (var s in studentsToCopy)
                {
                    string insertSQL = @"INSERT INTO student_info 
                (Nomer_paketa, Pol, Kod, FIO, Otmetka_ov_otsytstvii, Sredniy_ball_attestata, 
                 Otsenka_po_russkomy_yaziky, Otsenka_po_matematike, Forma_obychenya, Group_name, Spisok_BPR_Id) 
                VALUES ('', @Pol, '', @FIO, '', '', '', '', @Forma, @GroupId, @BPRId)";

                    var insertCmd = new MySqlCommand(insertSQL, conn);
                    insertCmd.Parameters.AddWithValue("@Pol", s.Pol);
                    insertCmd.Parameters.AddWithValue("@FIO", s.FIO);
                    insertCmd.Parameters.AddWithValue("@Forma", s.Forma_obychenya);
                    insertCmd.Parameters.AddWithValue("@GroupId", GroupId.Value);
                    insertCmd.Parameters.AddWithValue("@BPRId", Id);
                    insertCmd.ExecuteNonQuery();
                }
            }
            finally
            {
                Connection.CloseConnection(conn);
            }
        }

        public string DisplayInfo
        {
            get
            {
                string groupName = GetGroupName();
                string lessonName = GetLessonName();
                return $"👥 {groupName} | 📚 {lessonName}";
            }
        }
    }
}