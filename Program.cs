using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace core_data_provider;

public class Program
{
    private static void Main(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
        string connectionString = config.GetConnectionString("MSSQLServer") ?? string.Empty;

        UsersSQLQuerySelectCommand(connectionString);
    }

    private static void UsersSQLQuerySelectCommand(string connectionString)
    {
        DataTable dataTable = new DataTable();
        SqlConnection connection = new SqlConnection(connectionString);
        const string sqlQuery = "SELECT * FROM Security.Users;";
        SqlCommand command = new(sqlQuery, connection)
        {
            CommandText = sqlQuery,
            CommandTimeout = 1000,
            CommandType = CommandType.Text
        };

        try
        {
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            dataTable.Load(reader);
            command.Dispose();
            connection.Close();

            List<User> users = new List<User>();
            foreach (DataRow item in dataTable.Rows)
            {
                User user = new()
                {
                    Id = GetValueOrDefault<Guid>(item, "Id"),
                    UserName = GetValueOrDefault<string>(item, "UserName"),
                    NormalizedUserName = GetValueOrDefault<string>(item, "NormalizedUserName"),
                    Email = GetValueOrDefault<string>(item, "Email"),
                    NormalizedEmail = GetValueOrDefault<string>(item, "NormalizedEmail"),
                    EmailConfirmed = GetValueOrDefault<bool>(item, "EmailConfirmed"),
                    PasswordHash = GetValueOrDefault<string>(item, "PasswordHash"),
                    SecurityStamp = GetValueOrDefault<string>(item, "SecurityStamp"),
                    ConcurrencyStamp = GetValueOrDefault<string>(item, "ConcurrencyStamp"),
                    PhoneNumber = GetValueOrDefault<string>(item, "PhoneNumber"),
                    PhoneNumberConfirmed = GetValueOrDefault<bool>(item, "PhoneNumberConfirmed"),
                    TwoFactorEnabled = GetValueOrDefault<bool>(item, "TwoFactorEnabled"),
                    LockoutEnd = GetValueOrDefault<DateTime>(item, "LockoutEnd"),
                    LockoutEnabled = GetValueOrDefault<bool>(item, "LockoutEnabled"),
                    AccessFailedCount = GetValueOrDefault<int>(item, "AccessFailedCount")
                };

                users.Add(user);
            }

            foreach (var user in users)
            {
                Console.WriteLine("============================================");
                Console.WriteLine("Id: " + user.Id);
                Console.WriteLine("UserName: " + user.UserName);
                Console.WriteLine("Email: " + user.Email);
                Console.WriteLine("Password: " + user.PasswordHash);
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
        }
    }

    public static T GetValueOrDefault<T>(DataRow row, string index, T defaultValue = default!)
    {
        return !row.IsNull(index) ? (T)row[index] : defaultValue;
    }
}