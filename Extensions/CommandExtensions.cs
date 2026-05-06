using System.Data;
using Npgsql;

namespace task_management_system_aca.Extensions;

public static class CommandExtensions
{
    public static void AddParameter(this IDbCommand command, string name, object? value)
    {
        if (command is NpgsqlCommand npgsqlCommand)
        {
            npgsqlCommand.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }
    }
}