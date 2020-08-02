using CrudTestAssignment.Api.Api.V1.Models;
using CrudTestAssignment.Ui.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(string serverUrl)
        {
            if (string.IsNullOrWhiteSpace(serverUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serverUrl));

            _httpClient = new HttpClient { BaseAddress = new Uri(serverUrl, UriKind.Absolute) };
        }

        public async Task<UserModel> CreateUserAsync(string userName)
        {
            var user = new UserModel { Name = userName };

            using var response = await _httpClient.PostAsJsonAsync("users", user);
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new BadRequestException("Username must be at least 5 characters long and must not be empty"),
                HttpStatusCode.Conflict => throw new ConflictException("User with this name already exist"),
                HttpStatusCode.InternalServerError => throw new ServerRequestException("ServerError"),
                HttpStatusCode.Created => await response.Content.ReadAsAsync<UserModel>(),
                _ => throw new ServerRequestException("Request exception")
            };
        }

        public async Task<UserModel> GetUserByNameAsync(string userName)
        {
            using var response = await _httpClient.GetAsync($"users/{Uri.EscapeDataString(userName)}");
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => throw new NotFoundException("User with this name not found"),
                HttpStatusCode.OK => await response.Content.ReadAsAsync<UserModel>(),
                _ => throw new ServerRequestException("Request exception")
            };
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            using var response = await _httpClient.GetAsync("users");
            return response.StatusCode switch
            {
                HttpStatusCode.NoContent => throw new NotFoundException("The database is empty"),
                HttpStatusCode.OK => await response.Content.ReadAsAsync<IEnumerable<UserModel>>(),
                _ => throw new ServerRequestException("Request exception")
            };
        }

        public async Task<UserModel> UpdateUserAsync(int userId, string newUserName)
        {
            var user = new UserModel { Name = newUserName };

            using var response = await _httpClient.PutAsJsonAsync($"users/{Uri.EscapeDataString(userId.ToString())}", user);
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new BadRequestException("Username must be at least 5 characters long and must not be empty"),
                HttpStatusCode.Conflict => throw new ConflictException("User with this name already exist"),
                HttpStatusCode.NotFound => throw new NotFoundException("User with this name not found"),
                HttpStatusCode.InternalServerError => throw new ServerRequestException("ServerError"),
                HttpStatusCode.OK => await response.Content.ReadAsAsync<UserModel>(),
                _ => throw new ServerRequestException("Request exception")
            };
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            using var response = await _httpClient.DeleteAsync($"users/{Uri.EscapeDataString(userId.ToString())}");
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => throw new NotFoundException("User not found"),
                HttpStatusCode.NoContent => true,
                _ => throw new ServerRequestException("Request exception")
            };
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}