using CrudTestAssignment.DAL.Exceptions;
using CrudTestAssignment.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace CrudTestAssignment.DAL
{
    public class Repository : IRepository
    {
        private readonly string _connectionString;

        private const string AddUserProcedureName = "AddUser";

        private const string GetByIdProcedureName = "GetUserById";

        private const string GetAllProcedureName = "GetAllUsers";

        private const string GetByNameProcedureName = "GetUserByName";

        private const string UpdateProcedureName = "UpdateUser";

        private const string DeleteProcedureName = @"DeleteUser";

        public Repository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public async Task<User> CreateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                await using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    await using (var command = new SqlCommand(AddUserProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("name", user.Name);
                        command.Parameters.AddWithValue("createdDate", user.CreatedDate);

                        var result = await command.ExecuteScalarAsync(cancellationToken);

                        user.Id = Convert.ToInt32(result);

                        return user;
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601)
                    throw new DuplicateUserNameException($"User with name {user.Name} already exist");
                
                throw;
            }
        }

        public async Task<User> GetByNameAsync(string userName, CancellationToken cancellationToken)
        {
            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await using (var command = new SqlCommand(GetByNameProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

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

        public async Task<User> GetByIdAsync(int userId, CancellationToken cancellationToken)
        {
            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await using (var command = new SqlCommand(GetByIdProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

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

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            var users = default(List<User>);

            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await using (var command = new SqlCommand(GetAllProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

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

                await using (var command = new SqlCommand(UpdateProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

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

                await using (var command = new SqlCommand(DeleteProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

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