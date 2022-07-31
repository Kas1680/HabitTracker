using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.Sqlite;

class Program
{
    static string connectionString = @"Data Source = Habit.db";

    static void Main(string[] args)
    {
        
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
        GetUserInput();
    }


    static void GetUserInput()
    {
        Console.Clear();
        bool closeApp = false;

        string displayMsg =
            $"\n\nMAIN MENU " +
            $"\nWhat would you like to do\n " +
            $"\n0 - Close application. " +
            $"\n1 - View All Records. " +
            $"\n2 - Insert Record." +
            $"\n3 - Delete Record." +
            $"\n4 - Update Record." +
            $"\nYour Input: ";

        while (!closeApp)
        {
            Console.WriteLine(displayMsg);
            var userInput = Console.ReadLine();

            switch (userInput.Trim())
            {
                case "0":
                    CloseApp();
                    break;
                case "1":
                    ViewRecord();
                    break;
                case "2":
                    InsertRecord();
                    break;
                case "3":
                    DeleteRecord();
                    break;
                case "4":
                    UpdateRecord();
                    break;
                default:
                    Console.WriteLine("Error: Invalid Input");
                    break;
            }
        }
    }

    static void CloseApp()
    {
        Console.WriteLine("Closing Application. GoodBye");
        Environment.Exit(0);
    }
    static void ViewRecord()
    {
        Console.Clear();
        using (var connection = new SqliteConnection(connectionString))
        {
            using(var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"SELECT * FROM yourHabit";

                List<Habit> habitList = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Habit newHabit = new Habit(reader.GetInt32(0), DateTime.ParseExact(reader.GetString(1), "dd-mm-yy", new CultureInfo("en-US")), reader.GetInt32(2));
                        habitList.Add(newHabit);
                    }

                    Console.WriteLine("ID\tDATE\t\tQUANTITY");
                    foreach(Habit habit in habitList)
                    {
                        string output = Convert.ToString(habit.GetID()) + "\t";
                        output += habit.GetDateTime().ToString("dd-mm-yy") + "\t";
                        output += habit.GetQuantity().ToString();
                        Console.WriteLine(output);    
                    }

                }
                else
                {
                    Console.WriteLine("Error: No record is found in table");
                }

                connection.Close();
            }
        }

    }
    private static void InsertRecord()
    {
        Console.WriteLine("\nInserting record: ");
        string date = GetDateInput();
        int habitQuantity = GetQuantity();

        using (var connection = new SqliteConnection(connectionString))
        {
            using(var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText =
                    $"INSERT INTO yourHabit(Date, Quantity) VALUES('{date}', {habitQuantity})";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }
    }

    internal static int GetQuantity()
    {
        Console.WriteLine("\nPlease enter a quantity: ");

        string qInput = Console.ReadLine();

        if (qInput == "0") GetUserInput();

        int finalInput = Convert.ToInt32(qInput);
        return finalInput;
    }
    internal static string GetDateInput()
    {
        Console.WriteLine("\nPlease insert the date: (Format: dd-mm-yy). Type 0 to return to main menu");

        string dateInput = Console.ReadLine();
        
        if(dateInput == "0") GetUserInput();

        return dateInput;
    }

    

    static void DeleteRecord()
    {

    }

    static void UpdateRecord()
    {

    }

  
}

public class Habit
{
    public int ID { get; set; }
    public DateTime Date { get; set; }
    public int habitQuantity { get; set; }


    public Habit(int ID, DateTime Date, int habitQuantity)
    {
        this.ID = ID;
        this.Date = Date;
        this.habitQuantity = habitQuantity;
    }

    public int GetID()
    {
        return this.ID;
    }
    public DateTime GetDateTime()
    {
        return this.Date;
    }

    public int GetQuantity()
    {
        return this.habitQuantity;
    }
}

