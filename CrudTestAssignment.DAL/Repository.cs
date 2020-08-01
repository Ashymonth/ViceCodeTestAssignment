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

        public Repository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public async Task<UserEntity> CreateAsync(UserEntity user, CancellationToken cancellationToken)
        {
            try
            {
                user.CreatedDate = DateTime.Now;

                await using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    await using (var command = new SqlCommand(StoredProcedures.Users.AddUserProcedureName, connection))
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
                    throw new DuplicateUserNameException($"UserEntity with name {user.Name} already exist");
                
                throw;
            }
        }

        public async Task<UserEntity> GetByNameAsync(string userName, CancellationToken cancellationToken)
        {
            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await using (var command = new SqlCommand(StoredProcedures.Users.GetByNameProcedureName, connection))
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

        public async Task<UserEntity> GetByIdAsync(int userId, CancellationToken cancellationToken)
        {
            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await using (var command = new SqlCommand(StoredProcedures.Users.GetByIdProcedureName, connection))
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

        public async Task<IEnumerable<UserEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            var users = default(List<UserEntity>);

            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await using (var command = new SqlCommand(StoredProcedures.Users.GetAllProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await using (var result = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        if (result.HasRows)
                        {
                            users = new List<UserEntity>(result.FieldCount);
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

                await using (var command = new SqlCommand(StoredProcedures.Users.UpdateProcedureName, connection))
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

                await using (var command = new SqlCommand(StoredProcedures.Users.DeleteProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("id", userId);

                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }

        private UserEntity GetUser(IDataRecord dataRecord)
        {
            var user = new UserEntity
            {
                Id = dataRecord.GetInt32(dataRecord.GetOrdinal(nameof(UserEntity.Id))),
                Name = dataRecord.GetString(dataRecord.GetOrdinal(nameof(UserEntity.Name))),
                CreatedDate = dataRecord.GetDateTime(dataRecord.GetOrdinal(nameof(UserEntity.CreatedDate)))
            };

            return user;
        }
    }
}