using CrudTestAssignment.DAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CrudTestAssignment.Ui.Services
{
    public interface IApiService
    {
        Task<User> CreateUserAsync(string userName);

        Task<User> GetUserByNameAsync(string userName);

        Task<IEnumerable<User>> GetUsersAsync();

        Task<User> UpdateUserAsync(int userId, string newUserName);

        Task<bool> DeleteUserAsync(int userId);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(GetRequestAddress()) };
        }

        public async Task<User> CreateUserAsync(string userName)
        {
            var user = new UserViewModel { Name = userName };

            var response = await _httpClient.PostAsJsonAsync("/users", user);
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => null,
                HttpStatusCode.OK => await response.Content.ReadAsAsync<User>(),
                _ => throw new HttpRequestException("Request exception")
            };
        }

        public async Task<User> GetUserByNameAsync(string userName)
        {
            var response = await _httpClient.GetAsync($"users/{Uri.EscapeDataString(userName)}");
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => null,
                HttpStatusCode.OK => await response.Content.ReadAsAsync<User>(),
                _ => throw new HttpRequestException("Request exception")
            };
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync("/users");
            return response.StatusCode switch
            {
                HttpStatusCode.NoContent => null,
                HttpStatusCode.OK => await response.Content.ReadAsAsync<IEnumerable<User>>(),
                _ => throw new HttpRequestException("Request exception")
            };
        }

        public async Task<User> UpdateUserAsync(int userId, string newUserName)
        {
            var user = new UserViewModel { Name = newUserName};

            var response = await _httpClient.PutAsJsonAsync($"/users/{Uri.EscapeDataString(userId.ToString())}", user);
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => null,
                HttpStatusCode.NotFound => null,
                HttpStatusCode.OK => await response.Content.ReadAsAsync<User>(),
                _ => throw new HttpRequestException("Request exception")
            };
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var response = await _httpClient.DeleteAsync($"/users/{Uri.EscapeDataString(userId.ToString())}");
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => false,
                HttpStatusCode.NoContent => true,
                _ => throw new HttpRequestException("Request exception")
            };
        }

        private static string GetRequestAddress()
        {
            return ConfigurationManager.AppSettings["localhost"];
        }
    }
}