Hi again! 🤠

This is a simple C# console application developed in .NET that allows users to register and store their details in an SQL server database. 

This was adapted for demonstrational purposes from a new user registration function I developed for a small greenfield site in C# within the .NET Framework.

---

Features:
- Collects user details: first name, last name, username, email, and password
- Masks password input with asterisks (*) for a privacy layer
- Encrypts the password using SHA256 hashing before storage
- Saves user data to a SQL Server database using parameterised queries to prevent SQL injection. 
- Handles errors for issues like duplicate usernames and emails, and database connection issues 
- Easily extendable console application
  
---

Setup Instructions:

1. Have SQL Server Express installed: https://www.microsoft.com/en-gb/download/details.aspx?id=101064
2. Have SQL Server Management Studio (SSMS) installed: https://learn.microsoft.com/en-us/ssms/download-sql-server-management-studio-ssms
3. Download or clone this repository: https://github.com/pjakovleva/NewUserDatabase.git
4. Set up a database:
- Open SQL Server Management Studio (SSMS)
- Connect to your SQL Server instance, which should look like this: DESKTOP-XYZ\SQLEXPRESS
- Create a new database by entering "CREATE DATABASE DatabaseName;" in the query handler, followed by F5
- Use the new database and create a Users table by entering:
```
USE DatabaseName;
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    Username NVARCHAR(50) UNIQUE,
    Email NVARCHAR(100) UNIQUE,
    PasswordHash NVARCHAR(64)
);
``` 
5. Open Program.cs in Visual Studio
6. Update the connectionString with your SQL Server instance name and database name:
```
string connectionString = "Server=DESKTOP-XYZ\\SQLEXPRESS;Database=DatabaseName;Integrated Security=True;TrustServerCertificate=True;";
```
7. Ensure you have Microsoft.Data.SqlClient installed via NuGet Packages
8. Press F5 or Ctrl + F5 in Visual Studio window to run
9. The application should run and you may enter user details as requested 
10. If successful, you'll see a "User registered successfully!" message, like this:
![Screenshot 2025-05-12 191831](https://github.com/user-attachments/assets/2e4e4790-0b78-444c-8053-3e41a92e0824)
11. Check the database in SSMS by clicking on:
```
Databases in the Object Explorer -> Your DatabaseName -> Tables -> Right click on dbo.Users -> Choose "Select Top 1000 Rows"
```
12. You should see something like this:
![database](https://github.com/user-attachments/assets/51f0094e-0b12-45b5-b3fb-1cd465675c90)

13. If new user registration was unsuccessful, the console will notify you if there was an error, like this:
![exception](https://github.com/user-attachments/assets/93f3f252-9b91-4f29-8daf-e52a4e835cca)
