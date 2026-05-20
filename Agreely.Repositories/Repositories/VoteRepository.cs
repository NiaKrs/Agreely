using Agreely.Domain;
using Agreely.Repositories.Entities;
using Agreely.Repositories.Interfaces;
using Agreely.Repositories.Mappers;
using Microsoft.Data.SqlClient;

namespace Agreely.Repositories.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly string _connectionString;

        public VoteRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CastVote(AlignmentVote vote)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO [AlignmentVote]
                                (CommitmentVersionId, UserId, Vote, CreatedAt)
                                VALUES
                                (@CommitmentVersionId, @UserId, @Vote, GETDATE());";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CommitmentVersionId", vote.CommitmentVersionId);
                command.Parameters.AddWithValue("@UserId", vote.UserId);
                command.Parameters.AddWithValue("@Vote", vote.Vote);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public AlignmentVote? GetVote(int commitmentVersionId, int userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM [AlignmentVote]
                                WHERE CommitmentVersionId = @CommitmentVersionId AND UserId = @UserId;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CommitmentVersionId", commitmentVersionId);
                command.Parameters.AddWithValue("@UserId", userId);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var entity = new AlignmentVoteEntity
                        {
                            VoteId = (int)reader["VoteId"],
                            CommitmentVersionId = (int)reader["CommitmentVersionId"],
                            UserId = (int)reader["UserId"],
                            Vote = (int)reader["Vote"],
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        };
                        return AlignmentVoteMapper.ToDomain(entity);
                    }
                }
            }
            return null;
        }

        public void UpdateVote(AlignmentVote newVote)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE [AlignmentVote]
                                SET Vote = @Vote, CreatedAt = GETDATE()
                                WHERE CommitmentVersionId = @CommitmentVersionId AND UserId = @UserId;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CommitmentVersionId", newVote.CommitmentVersionId);
                command.Parameters.AddWithValue("@UserId", newVote.UserId);
                command.Parameters.AddWithValue("@Vote", newVote.Vote);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<AlignmentVote> GetVotesByVersion(int commitmentVersionId)
        {
            var votes = new List<AlignmentVote>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM [AlignmentVote]
                                WHERE CommitmentVersionId = @CommitmentVersionId;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CommitmentVersionId", commitmentVersionId);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var entity = new AlignmentVoteEntity
                        {
                            VoteId = (int)reader["VoteId"],
                            CommitmentVersionId = (int)reader["CommitmentVersionId"],
                            UserId = (int)reader["UserId"],
                            Vote = (int)reader["Vote"],
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        };
                        votes.Add(AlignmentVoteMapper.ToDomain(entity));
                    }
                }
            }
            return votes;
        }
    }
}
