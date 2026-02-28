using Microsoft.Data.SqlClient;
using System.Data;

namespace NotesApp.API.Data;

public class DbConnectionFactory(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("DefaultConnection")!;

    public IDbConnection Create() => new SqlConnection(_connectionString);
}