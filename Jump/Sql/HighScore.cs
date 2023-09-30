using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;
using static System.Formats.Asn1.AsnWriter;

namespace Jump.Sql
{
    public class HighScore
    {
        private static readonly SqliteConnection connection = new($"Data Source=HighScore.db");
        public HighScore()
        {
            connection.Open();

            var create = connection.CreateCommand();
            create.CommandText = @"
                CREATE TABLE IF NOT EXISTS HIGHSCORE (
                    NAME TEXT,
                    SCORE INT
                )
            ";
            create.ExecuteNonQuery();
        }

        public class HighScoreOwner
        {
            public string? name { get; set; }
            public int score { get; set; }
        };

        private void GetName(ref List<string> listname)
        {
            var getlistname = connection.CreateCommand();
            getlistname.CommandText = @"SELECT NAME FROM HIGHSCORE";

            var nameobj = new object[1];
            using (var reader = getlistname.ExecuteReader())
            {
                while (reader.Read())
                {
                    reader.GetValues(nameobj);
                    listname.Add(Convert.ToString(nameobj[0])!);
                }
            }
        }

        public void InsertScore(string name, int score)
        {
            List<string> listname = new List<string>();
            GetName(ref listname);

            foreach (var item in listname)
            {
                if (item == name)
                {
                    UpdateScore(name, score);
                    return;
                }
            }

            InsertNewScore(name, score);
        }

        public void UpdateScore(string name, int score)
        {
            using (var transaction = connection.BeginTransaction())
            {
                var update = new SqliteCommand
                    ("UPDATE HIGHSCORE " +
                    "SET SCORE = @score " +
                    "WHERE NAME = @name",
                connection, transaction
                    );
                update.Parameters.AddWithValue("@score", score);
                update.Parameters.AddWithValue("@name", name);

                update.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        public void InsertNewScore(string name, int score)
        {
            using (var transaction =  connection.BeginTransaction())
            {
                var insert = new SqliteCommand("INSERT INTO HIGHSCORE VALUES(@name, @score)", connection, transaction);
                insert.Parameters.AddWithValue("@name", name);
                insert.Parameters.AddWithValue("@score", score);
                
                insert.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        public void GetScore(ref List<HighScoreOwner> listhighscore)
        {
            var getscore = connection.CreateCommand();
            getscore.CommandText = @"
                    SELECT * FROM HIGHSCORE
                    ORDER BY SCORE DESC
                ";

            var highscore = new object[2];

            using (var reader = getscore.ExecuteReader())
            {
                listhighscore.Clear();
                int index = 0;

                while (reader.Read())
                {
                    HighScoreOwner highscoreowner = new HighScoreOwner();
                    listhighscore.Add(highscoreowner);
                    reader.GetValues(highscore);

                    listhighscore[index].name = Convert.ToString(highscore[0]);
                    listhighscore[index].score = Convert.ToInt32(highscore[1]);

                    index++;
                }
            }
        }
    }
}
