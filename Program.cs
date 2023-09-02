using System.Data.OleDb;
using Microsoft.Extensions.Configuration;

public class Program
{
    private static void Main(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        string connectionString = config.GetConnectionString("MSSQLServer") ?? string.Empty;
        Console.WriteLine(connectionString);

        //OleDbConnection connection = new OleDbConnection(connectionString);
    }
}