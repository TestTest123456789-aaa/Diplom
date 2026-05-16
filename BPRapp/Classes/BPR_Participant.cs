using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPRapp.Classes
{
    public class BPR_Participant
    {
        public int Id { get; set; }
        public int BPR_Id { get; set; }
        public int StudentId { get; set; }
        public string Nomer_paketa { get; set; }
        public string Kod { get; set; }
        public string Otmetka_ov_otsytstvii { get; set; }
        public string Sredniy_ball_attestata { get; set; }
        public string Otsenka_po_russkomy_yaziky { get; set; }
        public string Otsenka_po_matematike { get; set; }

        public BPR_Participant(int id, int bprId, int studentId, string paket, string kod,
                               string otmetka, string ball, string rus, string math)
        {
            Id = id; BPR_Id = bprId; StudentId = studentId;
            Nomer_paketa = paket; Kod = kod; Otmetka_ov_otsytstvii = otmetka;
            Sredniy_ball_attestata = ball; Otsenka_po_russkomy_yaziky = rus; Otsenka_po_matematike = math;
        }

        public static List<BPR_Participant> SelectByBPR(int bprId)
        {
            List<BPR_Participant> list = new List<BPR_Participant>();
            string sql = "SELECT * FROM `bpr_participants` WHERE `BPR_Id` = @BPRId;";
            MySqlConnection conn = Connection.OpenConnection();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BPRId", bprId);

            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new BPR_Participant(
                    reader.GetInt32("Id"),
                    reader.GetInt32("BPR_Id"),
                    reader.GetInt32("StudentId"),
                    reader.IsDBNull(3) ? "" : reader.GetString(3),
                    reader.IsDBNull(4) ? "" : reader.GetString(4),
                    reader.IsDBNull(5) ? "" : reader.GetString(5),
                    reader.IsDBNull(6) ? "" : reader.GetString(6),
                    reader.IsDBNull(7) ? "" : reader.GetString(7),
                    reader.IsDBNull(8) ? "" : reader.GetString(8)
                ));
            }
            reader.Close();
            Connection.CloseConnection(conn);
            return list;
        }

        public void Add()
        {
            string sql = "INSERT INTO `bpr_participants`(`BPR_Id`,`StudentId`,`Nomer_paketa`,`Kod`,`Otmetka_ov_otsytstvii`,`Sredniy_ball_attestata`,`Otsenka_po_russkomy_yaziky`,`Otsenka_po_matematike`) VALUES(@BPR,@SID,@Paket,@Kod,@Otmetka,@Ball,@Rus,@Math)";
            MySqlConnection conn = Connection.OpenConnection();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BPR", BPR_Id);
            cmd.Parameters.AddWithValue("@SID", StudentId);
            cmd.Parameters.AddWithValue("@Paket", Nomer_paketa ?? "");
            cmd.Parameters.AddWithValue("@Kod", Kod ?? "");
            cmd.Parameters.AddWithValue("@Otmetka", Otmetka_ov_otsytstvii ?? "");
            cmd.Parameters.AddWithValue("@Ball", Sredniy_ball_attestata ?? "");
            cmd.Parameters.AddWithValue("@Rus", Otsenka_po_russkomy_yaziky ?? "");
            cmd.Parameters.AddWithValue("@Math", Otsenka_po_matematike ?? "");
            cmd.ExecuteNonQuery();
            Connection.CloseConnection(conn);
        }

        public void Update()
        {
            string sql = "UPDATE `bpr_participants` SET `Nomer_paketa`=@Paket,`Kod`=@Kod,`Otmetka_ov_otsytstvii`=@Otmetka,`Sredniy_ball_attestata`=@Ball,`Otsenka_po_russkomy_yaziky`=@Rus,`Otsenka_po_matematike`=@Math WHERE `Id`=@Id";
            MySqlConnection conn = Connection.OpenConnection();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Paket", Nomer_paketa ?? "");
            cmd.Parameters.AddWithValue("@Kod", Kod ?? "");
            cmd.Parameters.AddWithValue("@Otmetka", Otmetka_ov_otsytstvii ?? "");
            cmd.Parameters.AddWithValue("@Ball", Sredniy_ball_attestata ?? "");
            cmd.Parameters.AddWithValue("@Rus", Otsenka_po_russkomy_yaziky ?? "");
            cmd.Parameters.AddWithValue("@Math", Otsenka_po_matematike ?? "");
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.ExecuteNonQuery();
            Connection.CloseConnection(conn);
        }

        public void Delete()
        {
            string sql = "DELETE FROM `bpr_participants` WHERE `Id` = @Id";
            MySqlConnection conn = Connection.OpenConnection();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.ExecuteNonQuery();
            Connection.CloseConnection(conn);
        }
    }
}