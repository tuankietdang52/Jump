using System;
using System.Collections.Generic;
using System.Linq;
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
                    SCORE INT PRIMARY KEY,
                    NAME TEXT
                )
            ";
            create.ExecuteNonQuery();
        }

        public void InsertScore(int score, string name)
        {
            using (var transaction =  connection.BeginTransaction())
            {
                var insert = new SqliteCommand("INSERT INTO HIGHSCORE VALUES(@score, @name)", connection);
                insert.Parameters.AddWithValue("@score", score);
                insert.Parameters.AddWithValue("@name", name);
                
                insert.ExecuteNonQuery();
                transaction.Commit();
            }
        }
    }
}
