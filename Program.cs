using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.Sqlite;
using HabitTracker;
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
        // Display the menu and await user input for CRUD operation
        Console.Clear();
        bool closeApp = false;

        string displayMsg =
            $"\n\nMAIN MENU " +
            $"\nWhat would you like to do?\n " +
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
                    closeApp = true;
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
        if (closeApp) CloseApp();
    }

    //-------------------------------------

    static void CloseApp()
    {
        Console.WriteLine("Closing Application. Goodbye");
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

                // Upload all records to a List using reader. Then print each of them.
                if (reader.HasRows)
                {
                     
                    while (reader.Read())
                    {
                        Habit newHabit = new Habit(reader.GetInt32(0), DateTime.ParseExact(reader.GetString(1), "dd-mm-yy", new CultureInfo("en-US")), reader.GetInt32(2));
                        habitList.Add(newHabit);
                    }

                    DateTime earliest = habitList[0].GetDateTime();
                    DateTime latest = habitList.Last().GetDateTime();
                    int total = 0;
                    

                    Console.WriteLine("ID\tDATE\t\tQUANTITY");
                    foreach (Habit habit in habitList)
                    {
                        // This part print out each record
                        string output = Convert.ToString(habit.GetID()) + "\t";
                        output += habit.GetDateTime().ToString("dd-mm-yy") + "\t";
                        output += habit.GetQuantity().ToString();
                        Console.WriteLine(output);

                        // This part try to find the earliest and latest record and sum quantity count
                        total += habit.GetQuantity();
                        DateTime check = habit.GetDateTime();
                        earliest = DateTime.Compare(earliest, check) <= 0? earliest : check ;
                        latest = DateTime.Compare(latest, check) >= 0 ? latest : check;
                    }

                    // Print the start and end date and total amount of quantity
                    

                    Console.WriteLine($"\nYour habit start at '{earliest.Day}, {earliest.Month}, {earliest.Year}' and end at '{latest.Day}, {latest.Month}, {latest.Year}'");
                    Console.WriteLine($"You total habit is: {total}");
                

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
                    $@"IF NOT EXISTS(SELECT * FROM yourHabit WHERE DATE = '{date})'
                    BEGIN
                    INSERT INTO yourHabit(Date, Quantity) VALUES('{date}', {habitQuantity})
                    END";

                tableCmd.ExecuteNonQuery();
            }
        }
    }
   
    // Helper to receive a date input from user with validation
    internal static string GetDateInput()
    {
        Console.WriteLine($"\nPlease insert the date (Format: dd-mm-yy). {exitMsg}: ");
        string? dateInput = Console.ReadLine();
        if (dateInput == "0") GetUserInput();

        while (!DateTime.TryParseExact(dateInput, "dd-mm-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
        {
            Console.WriteLine("\n\nInvalid Input. Please enter the correct date. Press 0 to exit.");
            dateInput = Console.ReadLine();
            if (dateInput == "0") GetUserInput();
        }

        return dateInput;
    }

    // Helper to receive quantity from user and validate input
    internal static int GetQuantity()
    {
        Console.WriteLine($"\nPlease enter a quantity. {exitMsg}: ");
        string qInput = Console.ReadLine();
        if (qInput == "0") GetUserInput();
        int finalInput;
        while (!int.TryParse(qInput, out finalInput) || Int32.Parse(qInput) < 0)
        {
            Console.WriteLine("Error: Invalid Input. Try Again.");
            qInput = Console.ReadLine();
        }
        return finalInput;
    }


    //-------------------------------------

    // Delete a record with the given ID or do nothing if ID does not exist
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
        var dateInput = Console.ReadLine();
        Console.WriteLine("\nEnter the quantity: ");
        var quanInput = Console.ReadLine();
        
        
        using (var connection = new SqliteConnection(connectionString))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText =
                    $"UPDATE yourHabit SET Quantity = '{quanInput}', Date = '{dateInput}' WHERE ID = '{IDInput}'";
                tableCmd.ExecuteNonQuery();
                Console.WriteLine("\nDelete Successful\n");
            }
        }
    }
}


