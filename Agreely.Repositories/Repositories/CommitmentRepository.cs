using Agreely.Domain;
using Agreely.Repositories.Entities;
using Agreely.Repositories.Interfaces;
using Agreely.Repositories.Mappers;
using Microsoft.Data.SqlClient;

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

        public List<Commitment> GetCommitmentsByGroupId(int groupId)
        {
            var commitments = new List<Commitment>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT CommitmentId, GroupId, CreatedByUserId, Title, Description, CreatedAt, UpdatedAt
                                 FROM [Commitment]
                                 WHERE GroupId = @GroupId
                                 ORDER BY CreatedAt DESC";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@GroupId", groupId);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var entity = new CommitmentEntity
                    {
                        CommitmentId = Convert.ToInt32(reader["CommitmentId"]),
                        GroupId = Convert.ToInt32(reader["GroupId"]),
                        CreatedByUserId = Convert.ToInt32(reader["CreatedByUserId"]),
                        Title = reader["Title"].ToString()!,
                        Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                    };
                    commitments.Add(CommitmentMapper.ToDomain(entity));
                }
            }
            return commitments;
        }

        public Commitment? GetCommitmentById(int commitmentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT CommitmentId, GroupId, CreatedByUserId, Title, Description, CreatedAt, UpdatedAt
                                 FROM [Commitment]
                                 WHERE CommitmentId = @CommitmentId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CommitmentId", commitmentId);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var entity = new CommitmentEntity
                    {
                        CommitmentId = Convert.ToInt32(reader["CommitmentId"]),
                        GroupId = Convert.ToInt32(reader["GroupId"]),
                        CreatedByUserId = Convert.ToInt32(reader["CreatedByUserId"]),
                        Title = reader["Title"].ToString()!,
                        Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                    };
                    return CommitmentMapper.ToDomain(entity);
                }
            }
            return null;
        }

        public void UpdateCommitment(Commitment commitment)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE [Commitment]
                                 SET Title = @Title, Description = @Description, UpdatedAt = GETDATE()
                                 WHERE CommitmentId = @CommitmentId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Title", commitment.Title);
                command.Parameters.AddWithValue("@Description", (object?)commitment.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@CommitmentId", commitment.CommitmentId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteCommitment(int commitmentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"DELETE FROM [Commitment] WHERE CommitmentId = @CommitmentId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CommitmentId", commitmentId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}