using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

class Program
{
    static string connectionString = "Server=localhost;User Id=newuser;Database=IMDB;Integrated Security=true;";

    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Movie Database Console App!");
        while (true)
        {
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Search for a movie title");
            Console.WriteLine("2. Search for a person");
            Console.WriteLine("3. Add a movie");
            Console.WriteLine("4. Add a person");
            Console.WriteLine("5. Update/Delete movie information");
            Console.WriteLine("6. Exit");

            int choice = GetChoice(1, 6);

            switch (choice)
            {
                case 1:
                    SearchMovieByTitleUsingView();
                    break;
                case 2:
                    SearchPerson();
                    break;
                case 3:
                    AddMovie();
                    break;
                case 4:
                    AddPerson();
                    break;
                case 5:
                    UpdateOrDeleteMovie();
                    break;
                case 6:
                    return;
            }
        }
    }

    static void SearchMovieByTitleUsingView()
    {
        Console.WriteLine("Enter the movie title (use % as a wildcard):");
        string searchTerm = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT tconst, primaryTitle FROM MovieSearch WHERE primaryTitle LIKE @searchTerm ORDER BY primaryTitle";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    Console.WriteLine("Search results for movies:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["tconst"]}, {reader["primaryTitle"]}");
                    }
                    Console.WriteLine("Do you wish to get more information?");
                    Console.WriteLine("1. Yes");
                    Console.WriteLine("2. No");
                    reader.Close();
                    int choice = GetChoice(1, 2);
                    if (choice == 1)
                    {
                        Console.WriteLine("Enter the 'tconst' of the movie to explore more options:");
                        string selectedMovieTconst = Console.ReadLine();

                        DisplayMoreMovieOptions(connection, selectedMovieTconst);
                    }
                    else if (choice == 2) 
                    {
                        Console.WriteLine("Continuing");
                    }
                }
                else
                {
                    Console.WriteLine("No movies found.");
                    Thread.Sleep(1000);
                    Console.Clear();
                }
            }
        }
        Console.ReadKey();
        Console.Clear();
    }
    static void DisplayMoreMovieOptions(SqlConnection connection, string selectedMovieTconst)
    {
        using (SqlCommand detailsCommand = new SqlCommand("SELECT * FROM GetMovieDetails(@tconst)", connection))
        {
            detailsCommand.Parameters.AddWithValue("@tconst", selectedMovieTconst);
            SqlDataReader detailsReader = detailsCommand.ExecuteReader();

            if (detailsReader.HasRows)
            {
                while (detailsReader.Read())
                {
                    Console.WriteLine("Movie Details:");
                    Console.WriteLine("Tconst: " + detailsReader["tconst"]);
                    Console.WriteLine("Primary title: " + detailsReader["primaryTitle"]);
                    Console.WriteLine("Is it for adults?: " + detailsReader["isAdult"]);
                    Console.WriteLine("Year: " + detailsReader["startYear"]);
                    Console.WriteLine("Runtime: " + detailsReader["runTimeMinutes"]);
                    Console.WriteLine("");
                    Console.WriteLine("Press any key to continue");
                }
            }
            else
            {
                Console.WriteLine("No details found for the selected movie.");
            }
        }
    }
    static void DisplayMorePersonOptions(SqlConnection connection, string selectedPersonNconst)
    {
        using (SqlCommand detailsCommand = new SqlCommand("SELECT * FROM GetPersonDetails(@nconst)", connection))
        {
            detailsCommand.Parameters.AddWithValue("@nconst", selectedPersonNconst);
            SqlDataReader detailsReader = detailsCommand.ExecuteReader();

            if (detailsReader.HasRows)
            {
                while (detailsReader.Read())
                {
                    Console.WriteLine("Movie Details:");
                    Console.WriteLine("Id: " + detailsReader["nconst"]);
                    Console.WriteLine("Name: " + detailsReader["primaryName"]);
                    Console.WriteLine("Birthyear: " + detailsReader["birthYear"]);
                    Console.WriteLine("DeathYear: " + detailsReader["deathYear"]);
                    Console.WriteLine("");
                    Console.WriteLine("Press any key to continue");
                }
            }
            else
            {
                Console.WriteLine("No details found for the selected person.");
            }
        }
    }



    static void SearchPerson()
    {
        Console.WriteLine("Enter the person's name (use % as a wildcard):");
        string searchTerm = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT nconst, primaryName FROM PersonSearch WHERE primaryName LIKE @searchTerm ORDER BY primaryName";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    Console.WriteLine("Search results for people:");
                    while (reader.Read())
                    {
                        Console.WriteLine(reader["primaryName"]);
                    }
                    Console.WriteLine("Do you wish to get more information?");
                    Console.WriteLine("1. Yes");
                    Console.WriteLine("2. No");
                    int choice = GetChoice(1, 2);
                    reader.Close();
                    if (choice == 1)
                    {
                        Console.WriteLine("Enter the 'nconst' of the movie to explore more options:");
                        string selectedPersonNconst = Console.ReadLine();

                        DisplayMorePersonOptions(connection, selectedPersonNconst);
                    }
                    else if (choice == 2)
                    {
                        Console.WriteLine("Continuing");
                    }
                }
                else
                {
                    Console.WriteLine("No people found.");
                }
            }
        }
        Console.ReadKey();
        Console.Clear();
    }
    static void AddMovie()
    {
        Console.WriteLine("Enter person details:");
        Console.Write("Primary title: ");
        string primaryTitle = Console.ReadLine();
        Console.Write("Original title: ");
        string originalTitle = Console.ReadLine();
        Console.Write("Is adult (true or false): ");
        string isAdult = Console.ReadLine();
        Console.Write("startyear: ");
        string startYear = Console.ReadLine();
        Console.Write("Endyear: ");
        string endYear = Console.ReadLine();
        Console.Write("Runtime: ");
        string runTimeMinutes = Console.ReadLine();
        bool.TryParse(isAdult, out bool isAdultBool);
        int.TryParse(startYear, out int startYearInt);
        int.TryParse(endYear, out int endYearInt);
        int.TryParse(runTimeMinutes, out int runTimeMinutesInt);

        AddMovieUsingStoredProcedure(originalTitle, primaryTitle, isAdultBool, startYearInt, endYearInt, runTimeMinutesInt);
    }
    static void AddMovieUsingStoredProcedure(string title, string originalTitle, bool isAdult, int startYear, int endYear, int runTimeMinutes)
    {
        int isAdultInt;
        if (isAdult)
        {
            isAdultInt = 1;
        } else
        {
            isAdultInt = 0;
        }
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand("AddMovie", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@primaryTitle", title);
                command.Parameters.AddWithValue("@originalTitle", originalTitle);
                command.Parameters.AddWithValue("@isAdult", isAdultInt);
                command.Parameters.AddWithValue("@startYear", startYear);
                command.Parameters.AddWithValue("@endYear", endYear);
                command.Parameters.AddWithValue("@runTimeMinutes", runTimeMinutes);
                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Movie added successfully.");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Failed to add the movie.");
                }
            }
        }
        Thread.Sleep(1000);
        Console.Clear();
    }


    static void AddPerson()
    {
        Console.WriteLine("Enter person details:");
        Console.Write("Name: ");
        string name = Console.ReadLine();
        Console.Write("BirthYear: ");
        string birthYearString = Console.ReadLine();
        int birthYear;
        int.TryParse(birthYearString, out birthYear);

        Console.Write("DeathYear (if not applicable, press Enter): ");
        string deathYearString = Console.ReadLine();
        int? deathYear = null;

        if (!string.IsNullOrEmpty(deathYearString))
        {
            int tempDeathYear;
            if (int.TryParse(deathYearString, out tempDeathYear))
            {
                deathYear = tempDeathYear; 
            }
        }

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand("AddPerson", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@primaryName", name);
                command.Parameters.AddWithValue("@birthYear", birthYear);

                if (deathYear.HasValue)
                {
                    command.Parameters.AddWithValue("@deathYear", deathYear);
                }
                else
                {
                    command.Parameters.AddWithValue("@deathYear", DBNull.Value); 
                }

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Person added successfully.");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Failed to add the person.");
                }
            }
        }
        Thread.Sleep(1000);
        Console.Clear();
    }

    static void UpdateOrDeleteMovie()
    {
        Console.WriteLine("Enter the movie title to update or delete:");
        string searchTerm = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT tconst, primaryTitle FROM Titles WHERE primaryTitle LIKE @searchTerm";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Tconst: {reader["tconst"]}, Title: {reader["primaryTitle"]}");
                    }
                    reader.Close();
                    Console.WriteLine("Enter the 'tconst' of the movie you want to update or delete:");
                    string tconst = Console.ReadLine();

                    Console.WriteLine("Select an option:");
                    Console.WriteLine("1. Update movie information");
                    Console.WriteLine("2. Delete movie");

                    int choice = GetChoice(1, 2);

                    if (choice == 1)
                    {
                        Console.WriteLine("Enter new movie title:");
                        string newTitle = Console.ReadLine();

                        string updateQuery = "UPDATE Titles SET primaryTitle = @newTitle WHERE tconst = @tconst";
                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@newTitle", newTitle);
                            updateCommand.Parameters.AddWithValue("@tconst", tconst);

                            int rowsAffected = updateCommand.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("Movie information updated successfully.");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine("Failed to update movie information.");
                            }
                        }
                    }
                    else if (choice == 2) 
                    {
                        using (SqlCommand deleteRelatedRecordsCommand = new SqlCommand("DELETE FROM TitleGenres WHERE tconst = @tconst", connection))
                        {
                            deleteRelatedRecordsCommand.Parameters.AddWithValue("@tconst", tconst);
                            deleteRelatedRecordsCommand.ExecuteNonQuery();
                        }

                        string deleteQuery = "DELETE FROM Titles WHERE tconst = @tconst";

                        using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@tconst", tconst);

                            int rowsAffected = deleteCommand.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("Movie deleted successfully.");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine("Failed to delete the movie.");
                            }
                        }
                    }

                }
                else
                {
                    Console.WriteLine("No movies found.");
                }
            }
        }
        Thread.Sleep(1000);
        Console.Clear();
    }

    static int GetChoice(int min, int max)
    {
        int choice;
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                if (choice >= min && choice <= max)
                {
                    return choice;
                }
            }
            Console.WriteLine("Invalid choice. Please enter a valid option.");
        }
    }
}
