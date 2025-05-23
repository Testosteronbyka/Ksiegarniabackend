using MySqlConnector;
using System;
using System.Threading.Tasks;

namespace Ksiegarniabackend.Utilities
{
    public class TestConnection
    {
        public static async Task TestDatabaseConnection()
        {
            var connectionString = "Server=mysql.titanaxe.com;Port=3306;Database=srv306153;User=srv306153;Password=x4YqYfMt;SslMode=Required;CharSet=utf8mb4;";
            
            try
            {
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();
                Console.WriteLine(" Połączenie z bazą danych udane!");
                
                using var command = new MySqlCommand("SELECT VERSION()", connection);
                var version = await command.ExecuteScalarAsync();
                Console.WriteLine($"Wersja MySQL: {version}");
                
                // Test tabeli Users
                using var testCommand = new MySqlCommand("SHOW TABLES LIKE 'Users'", connection);
                var tableExists = await testCommand.ExecuteScalarAsync();
                Console.WriteLine($"Tabela Users istnieje: {tableExists != null}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Błąd połączenia: {ex.Message}");
            }
        }
    }
}