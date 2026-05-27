using Agreely.Domain;
using Agreely.Domain.Enums;
using Agreely.Repositories.Entities;
using Agreely.Repositories.Interfaces;
using Agreely.Repositories.Mappers;
using Microsoft.Data.SqlClient;

namespace Agreely.Repositories.Repositories
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly string _connectionString;

        public ActivityLogRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void LogEvent(ActivityLog entry)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO ActivityLog (GroupId, UserId, EventType, OccuredAt, Description) VALUES (@GroupId, @UserId, @EventType, @OccuredAt, @Description)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GroupId", entry.GroupId);
                    command.Parameters.AddWithValue("@UserId", entry.UserId);
                    command.Parameters.AddWithValue("@EventType", (int)entry.EventType);
                    command.Parameters.AddWithValue("@OccuredAt", entry.OccuredAt);
                    command.Parameters.AddWithValue("@Description", entry.Description);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<ActivityLog> GetLogsByGroup(int groupId)
        {
            var logs = new List<ActivityLog>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT l.LogId, l.GroupId, l.UserId, l.EventType, l.Description, l.OccuredAt, u.FullName
                                FROM [ActivityLog] l
                                JOIN [User] u ON l.UserId = u.UserId
                                WHERE l.GroupId = @GroupId
                                ORDER BY l.OccuredAt DESC";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@GroupId", groupId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var entity = new ActivityLogEntity
                    {
                        LogId = Convert.ToInt32(reader["LogId"]),
                        GroupId = Convert.ToInt32(reader["GroupId"]),
                        UserId = Convert.ToInt32(reader["UserId"]),
                        EventType = Convert.ToInt32(reader["EventType"]),
                        OccuredAt = Convert.ToDateTime(reader["OccuredAt"]),
                        UserFullName = reader["FullName"].ToString() ?? string.Empty,
                        Description = reader["Description"].ToString() ?? string.Empty
                    };
                    logs.Add(ActivityLogMapper.ToDomain(entity));
                }
            }
            return logs;
        }
            
        
    }
}
