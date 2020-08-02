using CrudTestAssignment.Api.Api.V1.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CrudTestAssignment.Api.Integration.Test
{
    public static class SeedData
    {
        public static async Task<UserModel> CreateUser(HttpClient client, string methodToCall)
        {
            var user = new UserModel
            {
                Name = Guid.NewGuid().ToString()
            };

            var response = await client.PostAsJsonAsync(methodToCall, user);
            return await response.Content.ReadAsAsync<UserModel>();
        }
    }
}