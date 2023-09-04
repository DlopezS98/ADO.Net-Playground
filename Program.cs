using System.Data;
using System.Data.SqlClient;
using core_data_provider.Entities;
using core_data_provider.Repositories;
using core_data_provider.Services;
using Microsoft.Extensions.Configuration;

namespace core_data_provider;

public class Program
{
    private static IConfigurationRoot InitConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
    }
    static SqlConnection GetSqlConnection(IConfigurationRoot config)
    {
        string connectionString = config.GetConnectionString("MSSQLServer") ?? string.Empty;
        return new SqlConnection(connectionString);
    }

    private static void Main(string[] args)
    {
        IConfigurationRoot config = InitConfiguration();
        SqlConnection connection = GetSqlConnection(config);
        connection.Open();
        ShowAvailableEntities(connection);
        connection.Close();
    }

    public static void ShowAvailableEntities(SqlConnection connection)
    {
        int option;

        do
        {
            Console.Clear();
            Console.WriteLine("1. ToDos");
            Console.WriteLine("2. Users");
            Console.WriteLine("3. Salir");
            Console.WriteLine("Select an option: ");
            option = Convert.ToInt32(Console.ReadLine());

            switch (option)
            {
                case 1:
                    ShowToDosMenu(connection);
                    break;

                case 2:
                    Console.WriteLine("Not Implemented...");
                    WaitConsoleExecution();
                    break;

                case 3:
                    Console.WriteLine("Shutting down the application...");
                    WaitConsoleExecution();
                    break;

                default:
                    Console.WriteLine("Invalid Option...");
                    WaitConsoleExecution();
                    break;
            }

        } while (option != 3);
    }

    public static void ShowToDosMenu(SqlConnection connection)
    {
        int option;
        ToDosService service = new ToDosService(connection);

        do
        {
            Console.Clear();
            Console.WriteLine("================== Entity - ToDos ==================");
            Console.WriteLine("================== Available Operations ==================");
            Console.WriteLine("1. Create");
            Console.WriteLine("2. Read");
            Console.WriteLine("3. Update");
            Console.WriteLine("4. Delete");
            Console.WriteLine("5. Go Back");
            Console.WriteLine("Select an option: ");
            option = Convert.ToInt32(Console.ReadLine());

            switch (option)
            {
                case 1:
                    {
                        ToDo toDo = service.CreateAsync().GetAwaiter().GetResult();
                        Console.WriteLine("ToDo created");
                        Console.WriteLine("Id: " + toDo.Id);
                        Console.WriteLine("Title: " + toDo.Title);

                        WaitConsoleExecution();
                        break;
                    }

                case 2:
                    {
                        Console.Clear();
                        Console.WriteLine("Retrieving ToDos from the Database...");
                        List<ToDo> toDos = service.GetAll().GetAwaiter().GetResult();
                        Console.WriteLine("ToDos...");
                        foreach (ToDo toDo in toDos)
                        {
                            Console.WriteLine("====================================================");
                            Console.WriteLine("Id: " + toDo.Id);
                            Console.WriteLine("Title: " + toDo.Title);
                            Console.WriteLine("Description: " + toDo.Description ?? "Not Set");
                            Console.WriteLine("Status: " + toDo.Status);
                            Console.WriteLine("Created At: " + toDo.CreatedAt);
                            Console.WriteLine();
                        }

                        Console.WriteLine("====================================================");
                        Console.WriteLine();
                        WaitConsoleExecution();
                        break;
                    }

                case 3:
                    {
                        Console.WriteLine("Not Implemented...");
                        WaitConsoleExecution();
                        break;
                    }

                case 4:
                    {
                        Console.WriteLine("Not Implemented...");
                        WaitConsoleExecution();
                        break;
                    }

                case 5:
                    Console.WriteLine("\nGoing Back to previous menu...");
                    WaitConsoleExecution();
                    break;

                default:
                    Console.WriteLine("Invalid Option...");
                    WaitConsoleExecution();
                    break;
            }

        } while (option != 5);
    }

    private static void WaitConsoleExecution()
    {
        Console.WriteLine("Press Any Key...");
        Console.ReadLine();
    }
}