using CrudTestAssignment.DAL.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace CrudTestAssignment.DAL
{
    public interface IRepository
    {
        Task<int> CreateAsync(User user, CancellationToken cancellationToken);

        Task<User> GetByNameAsync(string userName, CancellationToken cancellationToken);

        Task<User> GetAsync(int userId, CancellationToken cancellationToken);

        Task<IEnumerable<User>> GetAsync(CancellationToken cancellationToken);

        Task UpdateAsync(int userId, string name, CancellationToken cancellationToken);

        Task DeleteAsync(int userId, CancellationToken cancellationToken);
       
    }

    public class Repository : IRepository
    {
        private readonly string _connectionString;

        private const string AddCommand = @"insert into [dbo].[Users] ([name],[createdDate])
                                            values(@name,@createdDate) select scope_identity();";

        private const string GetByIdCommand = @"select [id], [name], [createdDate] 
                                                from [dbo].[Users] 
                                                where [id] = @userId";

        private const string GetByNameCommand = @"select [id], [name], [createdDate] 
                                                  from [dbo].[Users] 
                                                  where [name] = @userName";

        private const string GetAllCommand = @"select [id], [name], [createdDate] 
                                               from [dbo].[Users]";

        private const string UpdateCommand = @"update [dbo].[Users]
                                               set [name] = @name
                                               where [id] = @id";

        private const string DeleteCommand = @"delete [dbo].[Users] where [id] = @id";

        public Repository(IOptions<ConnectionStringOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        public async Task<int> CreateAsync(User user, CancellationToken cancellationToken)
        {
            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await using (var command = new SqlCommand(AddCommand, connection))
                {
                    command.Parameters.AddWithValue("name", user.Name);
                    command.Parameters.AddWithValue("createdDate", user.CreatedDate);

                    var result = await command.ExecuteScalarAsync(cancellationToken);
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<User> GetByNameAsync(string userName, CancellationToken cancellationToken)
        {
            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await using (var command = new SqlCommand(GetByNameCommand, connection))
                {
                    command.Parameters.AddWithValue("userName", userName);

                    await using (var result = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        if (result.HasRows)
                        {
                            if (await result.ReadAsync(cancellationToken))
                            {
                                return GetUser(result);
                            }
                        }
                    }
                }
            }

            return null;
        }

        public async Task<User> GetAsync(int userId, CancellationToken cancellationToken)
        {
            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await using (var command = new SqlCommand(GetByIdCommand, connection))
                {
                    command.Parameters.AddWithValue("userId", userId);

                    await using (var result = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        if (result.HasRows)
                        {
                            if (await result.ReadAsync(cancellationToken))
                            {
                                return GetUser(result);
                            }
                        }
                    }
                }
            }

            return null;
        }

        public async Task<IEnumerable<User>> GetAsync(CancellationToken cancellationToken)
        {
            var users = default(List<User>);

            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await using (var command = new SqlCommand(GetAllCommand, connection))
                {
                    await using (var result = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        if (result.HasRows)
                        {
                            users = new List<User>(result.FieldCount);
                            while (await result.ReadAsync(cancellationToken))
                            {
                                var user = GetUser(result);
                                users.Add(user);
                            }
                        }
                    }
                }
            }

            return users;
        }

        public async Task UpdateAsync(int userId, string name, CancellationToken cancellationToken)
        {
            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await using (var command = new SqlCommand(UpdateCommand, connection))
                {
                    command.Parameters.AddWithValue("id", userId);

                    command.Parameters.AddWithValue("name", name);

                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }

        public async Task DeleteAsync(int userId, CancellationToken cancellationToken)
        {
            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await using (var command = new SqlCommand(DeleteCommand, connection))
                {
                    command.Parameters.AddWithValue("id", userId);

                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }

        private User GetUser(IDataRecord dataRecord)
        {
            var user = new User
            {
                Id = dataRecord.GetInt32(dataRecord.GetOrdinal(nameof(User.Id))),
                Name = dataRecord.GetString(dataRecord.GetOrdinal(nameof(User.Name))),
                CreatedDate = dataRecord.GetDateTime(dataRecord.GetOrdinal(nameof(User.CreatedDate)))
            };

            return user;
        }
    }
}