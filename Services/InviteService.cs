using Dapper;
using Npgsql;
using System.Diagnostics.CodeAnalysis;
using task_management_system_aca.Entities;

namespace task_management_system_aca.Services
{
    public class InviteService
    {
        private readonly string _connectionString;

        public InviteService()
        {
            _connectionString = "Host=localhost;Port=5432;Database=taskmanagement;Username=admin;Password=admin123";
        }

        public async Task<int> CreateInvitationAsync (BoardInvite invite)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                INSERT INTO board_invites (id, board_id, sender, receiver, status, created_at)
                VALUES (@Id, @BoardId, @Sender, @Receiver, @Status, @CreatedAt)
                RETURNING id;
               ";

            return await connection.QuerySingleAsync<int>(sql, invite);
        }
        public async Task<IEnumerable<BoardInvite>> GetBoardInvitesByBoardIdAsync(int boardId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM invites WHERE board_id = @boardId ORDER BY created_at DESC";

            return await connection.QueryAsync<BoardInvite>(sql, new { BoardId = boardId });
        }
    }
}
