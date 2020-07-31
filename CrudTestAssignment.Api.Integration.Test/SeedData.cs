using System;
using System.Net.Http;
using System.Threading.Tasks;
using CrudTestAssignment.DAL.Models;

namespace CrudTestAssignment.Api.Integration.Test
{
    public static class SeedData
    {
        public static async Task<User> CreateUser(HttpClient client, string methodToCall)
        {
            var user = new UserViewModel
            {
                Name = Guid.NewGuid().ToString()
            };

            var response = await client.PostAsJsonAsync(methodToCall, user);
            return await response.Content.ReadAsAsync<User>();
        }
    }
}