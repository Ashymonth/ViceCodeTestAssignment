using CrudTestAssignment.DAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CrudTestAssignment.Api.Api.V1.Models;

namespace CrudTestAssignment.Ui.Services
{
    public interface IApiService
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

        public ApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(GetRequestAddress()) };
        }

        public async Task<UserModel> CreateUserAsync(string userName)
        {
            var user = new UserModel { Name = userName };

            var response = await _httpClient.PostAsJsonAsync("api/v1/users", user);
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => null,
                HttpStatusCode.Created => await response.Content.ReadAsAsync<UserModel>(),
                _ => throw new HttpRequestException("Request exception")
            };
        }

        public async Task<UserModel> GetUserByNameAsync(string userName)
        {
            var response = await _httpClient.GetAsync($"api/v1/users/{Uri.EscapeDataString(userName)}");
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => null,
                HttpStatusCode.OK => await response.Content.ReadAsAsync<UserModel>(),
                _ => throw new HttpRequestException("Request exception")
            };
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync("api/v1/users");
            return response.StatusCode switch
            {
                HttpStatusCode.NoContent => null,
                HttpStatusCode.OK => await response.Content.ReadAsAsync<IEnumerable<UserModel>>(),
                _ => throw new HttpRequestException("Request exception")
            };
        }

        public async Task<UserModel> UpdateUserAsync(int userId, string newUserName)
        {
            var user = new UserModel { Name = newUserName};

            var response = await _httpClient.PutAsJsonAsync($"api/v1/users/{Uri.EscapeDataString(userId.ToString())}", user);
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => null,
                HttpStatusCode.NotFound => null,
                HttpStatusCode.OK => await response.Content.ReadAsAsync<UserModel>(),
                _ => throw new HttpRequestException("Request exception")
            };
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var response = await _httpClient.DeleteAsync($"api/v1/users/{Uri.EscapeDataString(userId.ToString())}");
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