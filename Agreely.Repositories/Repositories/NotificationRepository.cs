using Agreely.Domain;
using Agreely.Domain.Enums;
using Agreely.Repositories.Entities;
using Agreely.Repositories.Mappers;
using Microsoft.Data.SqlClient;

namespace Agreely.Repositories.Repositories
{
    public class NotificationRepository
    {
        private readonly string _connectionString;

        public NotificationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int CreateNotification(Notification notification)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"INSERT INTO [Notification] (UserId, GroupId, CommitmentId, HealthStatus, Message, IsRead, CreatedAt)
                     VALUES (@UserId, @GroupId, @CommitmentId, @HealthStatus, @Message, 0, GETDATE());
                     SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", notification.UserId);
            command.Parameters.AddWithValue("@GroupId", notification.GroupId);
            command.Parameters.AddWithValue("@CommitmentId", notification.CommitmentId);
            command.Parameters.AddWithValue("@HealthStatus", (int)notification.HealthStatus);
            command.Parameters.AddWithValue("@Message", notification.Message);
            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public List<Notification> GetNotificationsByUserId(int userId)
        {
            var notifications = new List<Notification>();
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"SELECT NotificationId, UserId, GroupId, CommitmentId, HealthStatus, Message, IsRead, CreatedAt
                             FROM [Notification]
                             WHERE UserId = @UserId
                             ORDER BY CreatedAt DESC";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var entity = new NotificationEntity
                {
                    NotificationId = Convert.ToInt32(reader["NotificationId"]),
                    UserId = Convert.ToInt32(reader["UserId"]),
                    GroupId = Convert.ToInt32(reader["GroupId"]),
                    CommitmentId = Convert.ToInt32(reader["CommitmentId"]),
                    HealthStatus = Convert.ToInt32(reader["HealthStatus"]),
                    Message = reader["Message"].ToString()!,
                    IsRead = Convert.ToBoolean(reader["IsRead"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                };
                notifications.Add(NotificationMapper.ToDomain(entity));
            }
            return notifications;
        }

        public void MarkAsRead(int notificationId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"UPDATE [Notification] SET IsRead = 1 WHERE NotificationId = @NotificationId";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@NotificationId", notificationId);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public int GetUnreadCount(int userId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"SELECT COUNT(*) FROM [Notification] WHERE UserId = @UserId AND IsRead = 0";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public bool NotificationExists(int userId, int commitmentId, HealthStatusValue healthStatus)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"SELECT COUNT(*) FROM [Notification] 
                             WHERE UserId = @UserId AND CommitmentId = @CommitmentId AND HealthStatus = @HealthStatus";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@CommitmentId", commitmentId);
            command.Parameters.AddWithValue("@HealthStatus", (int)healthStatus);
            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
    }
}
