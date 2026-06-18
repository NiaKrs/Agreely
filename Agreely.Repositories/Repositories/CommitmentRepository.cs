using Agreely.Domain;
using Agreely.Domain.Enums;
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

        public int InsertCommitment(Commitment commitment)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO [Commitment]
                        (GroupId, CreatedByUserId, CreatedAt, Status)
                        VALUES
                        (@GroupId, @CreatedByUserId, GETDATE(), @Status);
                        SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@GroupId", commitment.GroupId);
                command.Parameters.AddWithValue("@CreatedByUserId", commitment.CreatedByUserId);
                command.Parameters.AddWithValue("@Status", commitment.Status);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        

        public int CreateCommitmentVersion(CommitmentVersion version)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO [CommitmentVersion]
                                (CommitmentId, CreatedByUserId, Title, Description, CreatedAt, IsActive)
                                VALUES
                                (@CommitmentId, @CreatedByUserId, @Title, @Description, GETDATE(), @IsActive);
                                SELECT SCOPE_IDENTITY();";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CommitmentId", version.CommitmentId);
                command.Parameters.AddWithValue("@CreatedByUserId", version.CreatedByUserId);
                command.Parameters.AddWithValue("@Title", version.Title);
                command.Parameters.AddWithValue("@Description", (object?)version.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsActive", version.IsActive);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public CommitmentVersion? GetCurrentVersion(int commitmentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT Id, CommitmentId, CreatedByUserId, Title, Description, CreatedAt, IsActive
                                 FROM [CommitmentVersion]
                                 WHERE CommitmentId = @CommitmentId AND IsActive = 1";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CommitmentId", commitmentId);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var entity = new CommitmentVersionEntity
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        CommitmentId = Convert.ToInt32(reader["CommitmentId"]),
                        CreatedByUserId = Convert.ToInt32(reader["CreatedByUserId"]),
                        Title = reader["Title"].ToString()!,
                        Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    };
                    return CommitmentVersionMapper.ToDomain(entity);
                }
                return null;
            }
        }

        public void DeactivatePreviousVersions(int commitmentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE [CommitmentVersion]
                                 SET IsActive = 0
                                 WHERE CommitmentId = @CommitmentId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CommitmentId", commitmentId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void UpdateCommitmentStatus(int commitmentId, CommitmentStatus status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE [Commitment]
                                 SET Status = @Status
                                 WHERE CommitmentId = @CommitmentId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Status", (int)status);
                command.Parameters.AddWithValue("@CommitmentId", commitmentId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<Commitment> GetCommitmentsByGroupId(int groupId)
        {
            var commitments = new List<Commitment>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT CommitmentId, GroupId, CreatedByUserId, CreatedAt, Status
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
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        Status = (int)reader["Status"]
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
                string query = @"SELECT CommitmentId, GroupId, CreatedByUserId, CreatedAt, Status
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
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        Status = (int)reader["Status"]
                    };
                    return CommitmentMapper.ToDomain(entity);
                }
            }
            return null;
        }

        public void DeleteVotesByCommitmentId(int commitmentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"DELETE FROM [AlignmentVote]
                         WHERE CommitmentVersionId IN (
                             SELECT Id FROM [CommitmentVersion] WHERE CommitmentId = @CommitmentId
                         )";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CommitmentId", commitmentId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteVersionsByCommitmentId(int commitmentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"DELETE FROM [CommitmentVersion] WHERE CommitmentId = @CommitmentId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CommitmentId", commitmentId);
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

        public void DeleteNotificationsByCommitmentId(int commitmentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"DELETE FROM [Notification] WHERE CommitmentId = @CommitmentId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CommitmentId", commitmentId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<Commitment> GetAllCommitments()
        {
            var commitments = new List<Commitment>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT CommitmentId, GroupId, CreatedByUserId, CreatedAt, Status
                                 FROM [Commitment]";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var entity = new CommitmentEntity
                    {
                        CommitmentId = Convert.ToInt32(reader["CommitmentId"]),
                        GroupId = Convert.ToInt32(reader["GroupId"]),
                        CreatedByUserId = Convert.ToInt32(reader["CreatedByUserId"]),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        Status = (int)reader["Status"]
                    };
                    commitments.Add(CommitmentMapper.ToDomain(entity));
                }
            }
            return commitments;
        }
    }
}