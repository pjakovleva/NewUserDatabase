using System;
using Microsoft.Data.SqlClient; // Provides classes for SQL Server database access, for example: SqlConnection, SqlCommand
using System.Security.Cryptography; // Provides cryptographic services, including SHA256)
using System.Text; // Provides StringBuilder and encoding utilities (e.g., UTF8 encoding for hashing)

class Program
{
    // Main method: Entry point of the console application. This handles user input, password encryption, and database storage.
    static void Main(string[] args)
    {
        // Prompts the user to enter their first name
        // Using a null-coalescing operator ?? to default to empty string if there's no imput
        Console.Write("Enter Your First Name: ");
        string firstName = Console.ReadLine() ?? string.Empty;

        // Prompts the user for their last name
        Console.Write("Enter Your Last Name: ");
        string lastName = Console.ReadLine() ?? string.Empty;

        // Prompts for username, which must be unique in the database
        Console.Write("Enter a Username: ");
        string username = Console.ReadLine() ?? string.Empty;

        // Prompts for email, which must also be unique in the database
        Console.Write("Enter Email: ");
        string email = Console.ReadLine() ?? string.Empty;

        // Prompts for a password, which will be encrypted before storage
        Console.Write("Enter Password: ");

        // Using a custom method to mask the input with asterisks for security
        string password = ReadPasswordMasked();

        // Validating the password to ensure it's not null or empty. This prevents passing invalid data to EncryptPassword
        // If empty, console will display an error and exit the method
        if (string.IsNullOrEmpty(password))
        {
            Console.WriteLine("\nError: Password cannot be empty.");
            return;
        }

        // Encrypting the password using SHA256 hashing. This ensures the password is not stored in plain text in the database
        string passwordHash = EncryptPassword(password);

        // Defining the connection string, which connects to the SQL Server database
        // !!! Please replace DESKTOP-XYZ\\SQLEXPRESS with your server name (from SSMS) and DatabaseName with your database!!!
        string connectionString = "Server=DESKTOP-XYZ\\SQLEXPRESS;Database=DatabaseName;Integrated Security=True;TrustServerCertificate=True;";

        // Using a try-catch block to handle any potential errors. This ensures the application doesn't crash and provides feedback to the user
        try
        {
            // Creating a new SqlConnection to connect to the database
            // The 'using' statement ensures the connection is properly disposed of after use
            using (var connection = new SqlConnection(connectionString))
            {
                // Opening the database connection. This establishes a connection to the UserDB database on the specified server
                connection.Open();

                // Defining the SQL INSERT query to add a new user to the Users table
                string query = "INSERT INTO Users (FirstName, LastName, Username, Email, PasswordHash) VALUES (@FirstName, @LastName, @Username, @Email, @PasswordHash)";

                // Creating a new SqlCommand to execute the INSERT query
                // The 'using' statement ensures the command is disposed of after use
                using (var command = new SqlCommand(query, connection))
                {
                    // Adding parameters to the query to safely pass user input
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                    // Executing the query to insert the user into the database
                    command.ExecuteNonQuery();
                }

                // Informing the user that their registration was successful
                Console.WriteLine("\nUser registered successfully!");
            }
        }
        catch (SqlException ex)
        {
            // Catching SQL-specific errors, such as connection failures or constraint violations, for example: duplicate username/email in the database
            Console.WriteLine($"\nDatabase error: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Catching any other unexpected errors,for example: invalid operation
            Console.WriteLine($"\nUnexpected error: {ex.Message}");
        }
    }

    // This method registers password and displays asterisks for each character typed
    static string ReadPasswordMasked()
    {
        StringBuilder passwordBuilder = new StringBuilder();

        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            // When Enter is pressed, break the loop to show the password 
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            // When Backspace is pressed, it removes characters as its standard behaviour
            else if (key.Key == ConsoleKey.Backspace && passwordBuilder.Length > 0)
            {
                passwordBuilder.Length--;

                Console.Write("\b \b");
            }
            // Handling only printable characters, ignoring special characters like arrows or Ctrl to avoid unexpected behaviour
            else if (!char.IsControl(key.KeyChar))
            {
                passwordBuilder.Append(key.KeyChar);

                Console.Write("*");
            }
        }

        // Converting the StringBuilder to a string and returning the password
        return passwordBuilder.ToString();
    }

    // This method encrypts a normal string password using SHA256 hashing 
    // Parameters: password (string) - the plaintext password to encrypt
    static string EncryptPassword(string password)
    {
        // Validate the input to ensure it's not null or empty
        if (string.IsNullOrEmpty(password))
        {
            // Ensuring the password is not empty before proceeding with hashing
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        }

        // Using SHA256 to generate the hash of the password
        using (var sha256 = SHA256.Create())
        {
            // Converting the password string to a byte array using UTF-8 encoding
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Using StringBuilder to build the hexadecimal representation of the hash
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                // Converting each byte to a 2-digit hexadecimal string (e.g., 255 becomes "ff")
                // "x2" format ensures lowercase hex with leading zeros
                builder.Append(b.ToString("x2"));
            }

            // Returning the final hash as a string (64 characters long)
            return builder.ToString();
        }
    }
}