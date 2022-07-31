using System;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main(string[] args)
    {
        string connectionString = @"Data Source = Habit.db";

        CreateDatabase();

        void CreateDatabase()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                using (var tableCmd = connection.CreateCommand())
                {
                    connection.Open();
                    tableCmd.CommandText =
                        @"CREATE TABLE IF NOT EXISTS yourHabit(
                        ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                        Date TEXT,
                        Quantity INTEGER
                        )";

                    tableCmd.ExecuteNonQuery();
                }
            }
        }
    }
}