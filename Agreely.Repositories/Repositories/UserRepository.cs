using Agreely.Repositories.Entities;
using Agreely.Repositories.Interfaces;
using Agreely.Domain;
using Agreely.Repositories.Mappers;
using Microsoft.Data.SqlClient;

namespace Agreely.Repositories.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int CreateUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO [User] (FullName, Email, Password)
                                 VALUES (@FullName, @Email, @Password);
                                 SELECT SCOPE_IDENTITY();";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FullName", user.FullName);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.PasswordHash);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public User? GetUserByEmail(string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT UserId, FullName, Email, Password
                                 FROM [User]
                                 WHERE Email = @Email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var entity = new UserEntity
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        FullName = reader["FullName"].ToString()!,
                        Email = reader["Email"].ToString()!,
                        Password = reader["Password"].ToString()!
                    };
                    return UserMapper.ToDomain(entity);
                }
                return null;
            }
        }
    }
}
