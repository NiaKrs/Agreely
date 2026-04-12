using Agreely.Repositories.Models;
using Agreely.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace Agreely.Repositories.Repositories
{
    public class GroupMembershipRepository : IGroupMembershipRepository
    {
        private readonly string _connectionString;

        public GroupMembershipRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddMember(GroupMembership membership)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO [GroupMembership] (GroupId, UserId)
                                 VALUES (@GroupId, @UserId);";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@GroupId", membership.GroupId);
                command.Parameters.AddWithValue("@UserId", membership.UserId);
                

                connection.Open();
                command.ExecuteNonQuery();
                return;
            }
        }

        public bool IsMember(int groupId, int userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT COUNT(1) FROM [GroupMembership] 
                         WHERE GroupId = @GroupId AND UserId = @UserId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@GroupId", groupId);
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }


    }
}
