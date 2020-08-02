using CrudTestAssignment.Api.Api.V1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrudTestAssignment.Ui.Services
{
    public interface IApiService : IDisposable
    {
        Task<UserModel> CreateUserAsync(string userName);

        Task<UserModel> GetUserByNameAsync(string userName);

        Task<IEnumerable<UserModel>> GetUsersAsync();

        Task<UserModel> UpdateUserAsync(int userId, string newUserName);

        Task<bool> DeleteUserAsync(int userId);
    }
}