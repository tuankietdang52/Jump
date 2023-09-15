using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

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

        public void InsertScore(int score, string name)
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

                SortList(ref listhighscore);
            }
        }

        public void SortList(ref List<HighScoreOwner> listhighscore)
        {
            for (int i = 0; i < listhighscore.Count; i++)
            {
                for (int j = i + 1;  j < listhighscore.Count; j++)
                {
                    if (listhighscore[i].score > listhighscore[j].score) continue;
                    var temp = listhighscore[i];
                    listhighscore[i] = listhighscore[j];
                    listhighscore[j] = temp;
                }
            }
        }
    }
}
