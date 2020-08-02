using CrudTestAssignment.Api.Api.V1.Models;
using CrudTestAssignment.Ui.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CrudTestAssignment.Ui.Services
{
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
                HttpStatusCode.BadRequest => throw new BadRequestException(ErrorMessages.UserNameIsEmpty),
                HttpStatusCode.Conflict => throw new ConflictException(string.Concat(ErrorMessages.UserNameExistPlaceHolder,userName)),
                HttpStatusCode.InternalServerError => throw new ServerRequestException(ErrorMessages.ServerException),
                HttpStatusCode.Created => await response.Content.ReadAsAsync<UserModel>(),
                _ => throw new ServerRequestException(ErrorMessages.RequestException)
            };
        }

        public async Task<UserModel> GetUserByNameAsync(string userName)
        {
            using var response = await _httpClient.GetAsync($"users/{Uri.EscapeDataString(userName)}");
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => throw new NotFoundException(string.Format(ErrorMessages.UserNameNotFoundPlaceHolder,userName)),
                HttpStatusCode.InternalServerError => throw new ServerRequestException(ErrorMessages.ServerException),
                HttpStatusCode.OK => await response.Content.ReadAsAsync<UserModel>(),
                _ => throw new ServerRequestException(ErrorMessages.RequestException)
            };
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            using var response = await _httpClient.GetAsync("users");
            return response.StatusCode switch
            {
                HttpStatusCode.NoContent => throw new NotFoundException(ErrorMessages.DataBaseEmpty),
                HttpStatusCode.InternalServerError => throw new ServerRequestException(ErrorMessages.ServerException),
                HttpStatusCode.OK => await response.Content.ReadAsAsync<IEnumerable<UserModel>>(),
                _ => throw new ServerRequestException(ErrorMessages.RequestException)
            };
        }

        public async Task<UserModel> UpdateUserAsync(int userId, string newUserName)
        {
            var user = new UserModel { Name = newUserName };

            using var response = await _httpClient.PutAsJsonAsync($"users/{Uri.EscapeDataString(userId.ToString())}", user);
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new BadRequestException(ErrorMessages.UserNameIsEmpty),
                HttpStatusCode.Conflict => throw new ConflictException(string.Format(ErrorMessages.UserNameExistPlaceHolder,newUserName)),
                HttpStatusCode.NotFound => throw new NotFoundException(ErrorMessages.UserNameNotFound),
                HttpStatusCode.InternalServerError => throw new ServerRequestException(ErrorMessages.ServerException),
                HttpStatusCode.OK => await response.Content.ReadAsAsync<UserModel>(),
                _ => throw new ServerRequestException(ErrorMessages.RequestException)
            };
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            using var response = await _httpClient.DeleteAsync($"users/{Uri.EscapeDataString(userId.ToString())}");
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => throw new NotFoundException(ErrorMessages.UserNameNotFound),
                HttpStatusCode.InternalServerError => throw new ServerRequestException(ErrorMessages.ServerException),
                HttpStatusCode.NoContent => true,
                _ => throw new ServerRequestException(ErrorMessages.ServerException)
            };
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}