using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
namespace SCB;


class Program
{
    const string connectionString = "Server=localhost; User ID=root; Database=SCB";
    
    static void Main(string[] args)
    {
        IDbConnection db = new MySqlConnection(connectionString);
        var userId = 1;
        var users = db.Query<string>("select Name from Freshness where Id = @userId", new { userId });
        Console.WriteLine($"Name: {users.FirstOrDefault()}");
    }
}