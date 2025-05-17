using MySql.Data.MySqlClient;
using System.Collections.Generic;

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
        public string Group_name { get; set; }
        public Student_Info(int Id, string Nomer_paketa, string Pol, string Kod, string FIO, string Otmetka_ov_otsytstvii, 
            string Sredniy_ball_attestata, string Otsenka_po_russkomy_yaziky, string Otsenka_po_matematike, string Forma_obychenya, string Group_name)
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
            List<Student_Info> AllStudent_Info = new List<Student_Info>();
            string SQL = "SELECT * FROM `student_info`;";
            MySqlConnection connection = OpenConnection();
            MySqlDataReader Data = Query(SQL, connection);
            while (Data.Read())
            {
                AllStudent_Info.Add(new Student_Info(
                    Data.GetInt32(0),
                    Data.GetString(1),
                    Data.GetString(2),
                    Data.GetString(3),
                    Data.GetString(4),
                    Data.GetString(5),
                    Data.GetString(6),
                    Data.GetString(7),
                    Data.GetString(8),
                    Data.GetString(9),
                    Data.GetString(10)
                    ));
            }
            CloseConnection(connection);
            return AllStudent_Info;
        }
        public void Add()
        {
            string SQL = $"INSERT INTO `student_info`(`Nomer_paketa`, `Pol`, `Kod`, `FIO`, `Otmetka_ov_otsytstvii`, `Sredniy_ball_attestata`, " +
                         $"`Otsenka_po_russkomy_yaziky`, `Otsenka_po_matematike`, `Forma_obychenya`, `Group_name`) " +
                         $"VALUES ('{Nomer_paketa}','{Pol}','{Kod}','{FIO}','{Otmetka_ov_otsytstvii}','{Sredniy_ball_attestata}'," +
                         $"'{Otsenka_po_russkomy_yaziky}','{Otsenka_po_matematike}','{Forma_obychenya}','{Group_name}')";
            MySqlConnection connection = OpenConnection();
            Query(SQL, connection);
            CloseConnection(connection);
        }
        public void Update()
        {
            string SQL = $"UPDATE `student_info` SET `Nomer_paketa`='{Nomer_paketa}',`Pol`='{Pol}',`Kod`='{Kod}',`FIO`='{FIO}',`Otmetka_ov_otsytstvii`='{Otmetka_ov_otsytstvii}',`Sredniy_ball_attestata`='{Sredniy_ball_attestata}'," +
                         $"`Otsenka_po_russkomy_yaziky`='{Otsenka_po_russkomy_yaziky}',`Otsenka_po_matematike`='{Otsenka_po_matematike}',`Forma_obychenya`='{Forma_obychenya}',`Group_name`='{Group_name}' WHERE `Id`='{Id}'";
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
    }
}