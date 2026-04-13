using Microsoft.Data.SqlClient;
using Agreely.Repositories.Models;
using Agreely.Repositories.Interfaces;

namespace Agreely.Repositories.Repositories
{
    public class CommitmentRepository : ICommitmentRepository
    {
        private readonly string _connectionString;

        public CommitmentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int CreateCommitment(Commitment commitment)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO [Commitment] 
                                (GroupId, CreatedByUserId, Title, Description, CreatedAt, UpdatedAt)
                                VALUES 
                                (@GroupId, @CreatedByUserId, @Title, @Description, GETDATE(), GETDATE());
                                SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@GroupId", commitment.GroupId);
                command.Parameters.AddWithValue("@CreatedByUserId", commitment.CreatedByUserId);
                command.Parameters.AddWithValue("@Title", commitment.Title);
                command.Parameters.AddWithValue("@Description", (object?)commitment.Description ?? DBNull.Value);

                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}