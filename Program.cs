using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.Sqlite;

class Program
{
    /* Create a connection using a connection string as arguement.
       The program will also auto-create the Habit database if none exist.

    */
    static string connectionString = @"Data Source = Habit.db";
    static string exitMsg = "Type 0 to return to main menu";
    static void Main(string[] args)
    {

        CreateDatabase();

        void CreateDatabase()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                // Create a command to be send to database
                using (var tableCmd = connection.CreateCommand())
                {
                    connection.Open();
                    tableCmd.CommandText =
                        @"CREATE TABLE IF NOT EXISTS yourHabit(
                        ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                        Date TEXT,
                        Quantity INTEGER
                        )";

                    // Execute the given command
                    tableCmd.ExecuteNonQuery();

                }
            }
            // Don't need connection.Close() since using statement
            // autoamtically uses Dispose()
        }
        // Call the next method that display the menu and request user input
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
            $"\n4 - Update Record.\n" +
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

    //-------------------------------------

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
            using (var tableCmd = connection.CreateCommand())
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
                    foreach (Habit habit in habitList)
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
            }
        }
    }

    //-------------------------------------
    private static void InsertRecord()
    {
        Console.WriteLine("\nInserting record: ");
        string date = GetDateInput();
        int habitQuantity = GetQuantity();

        using (var connection = new SqliteConnection(connectionString))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText =
                    $"INSERT INTO yourHabit(Date, Quantity) VALUES('{date}', {habitQuantity})";

                tableCmd.ExecuteNonQuery();
             }
        }
    }

    internal static int GetQuantity()
    {
        Console.WriteLine($"\nPlease enter a quantity. {exitMsg}: ");

        string qInput = Console.ReadLine();

        if (qInput == "0") GetUserInput();

        int finalInput = Convert.ToInt32(qInput);
        return finalInput;
    }
    internal static string GetDateInput()
    {
        Console.WriteLine($"\nPlease insert the date (Format: dd-mm-yy). {exitMsg}: ");

        string dateInput = Console.ReadLine();

        if (dateInput == "0") GetUserInput();

        return dateInput;
    }


    //-------------------------------------

    static void DeleteRecord()
    {
        ViewRecord();
        Console.WriteLine("Deleting Record: ");
        Console.WriteLine($"\nEnter the ID of record you would like to remove. {exitMsg} ");
        var userInput = Console.ReadLine();


        using ( var connection = new SqliteConnection(connectionString))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"DELETE FROM yourHabit WHERE ID = '{userInput}'";
                tableCmd.ExecuteNonQuery();
                Console.WriteLine("\nDelete Successful\n");
            }
        }
    }


    //-------------------------------------

    static void UpdateRecord()
    {
        ViewRecord();
        Console.WriteLine("\n\nUpdating Record: ");
        Console.WriteLine("\nEnter the ID of the record you want to update: ");
        var IDInput = Console.ReadLine();
        Console.WriteLine("\nEnter the date in the format (dd-mm-yy): ");
        var quanInput = Console.ReadLine();
        Console.WriteLine("\nEnter the quantity: ");
        var dateInput = Console.ReadLine();
        
        
        using (var connection = new SqliteConnection(connectionString))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText =
                    $"UPDATE yourHabit SET Quantity = '{quanInput}', Date = '{dateInput}' " +
                    $"WHERE ID = '{IDInput}'";
            }
        }
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

