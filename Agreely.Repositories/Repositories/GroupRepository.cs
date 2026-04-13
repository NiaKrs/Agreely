using Agreely.Repositories.Models;
using Agreely.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace Agreely.Repositories.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly string _connectionString;

        public GroupRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int CreateGroup(Group group)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO [Group] (Name, Description, CreatedByUserId)
                                 VALUES (@Name, @Description, @CreatedByUserId);
                                 SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", group.Name);
                command.Parameters.AddWithValue("@Description", group.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedByUserId", group.CreatedByUserId);
                

                connection.Open();
                int newGroupId = Convert.ToInt32(command.ExecuteScalar());
                return newGroupId;
            }
        }

        public Group GetGroupById(int groupId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT GroupId, Name, Description, CreatedByUserId, CreatedAt 
                         FROM [Group] 
                         WHERE GroupId = @GroupId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@GroupId", groupId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Group
                    {
                        GroupId = Convert.ToInt32(reader["GroupId"]),
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                        CreatedByUserId = Convert.ToInt32(reader["CreatedByUserId"]),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                    };
                }

                return null;
            }
        }

        public int GetMemberCount(int groupId) 
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT COUNT(1) FROM [GroupMembership] 
                         WHERE GroupId = @GroupId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@GroupId", groupId);
                

                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count;
            }
        }

        public List<Group> GetGroupsByUserId(int userId)
        {
            var groups = new List<Group>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT g.GroupId, g.Name, g.Description, g.CreatedByUserId
                         FROM [Group] g
                         INNER JOIN [GroupMembership] gm ON g.GroupId = gm.GroupId
                         WHERE gm.UserId = @UserId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    groups.Add(new Group
                    {
                        GroupId = Convert.ToInt32(reader["GroupId"]),
                        Name = reader["Name"].ToString()!,
                        Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                        CreatedByUserId = Convert.ToInt32(reader["CreatedByUserId"])
                    });
                }
            }

            return groups;
        } 

    }
}
