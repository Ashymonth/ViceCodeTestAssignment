using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CrudTestAssignment.Api.Integration.Test.ControllersTest.UserController
{
    public class ControllerDeleteTest: IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ControllerDeleteTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task DeleteUser_UserExist_ShouldBe_OK()
        {
            //Arrange
            const string methodToCall = "/api/v1/users";

            var client = _factory.CreateClient();
            var user = await SeedData.CreateUser(client, methodToCall);

            //Act
            var response = await client.DeleteAsync($"{methodToCall}/{user.Id}");

            //Assert
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_UserNotExist_ShouldBe_NotFound()
        {
            //Arrange
            const string methodToCall = "/api/v1/users";
            const int invalidId = 0;

            var client = _factory.CreateClient();

            //Act
            var response = await client.DeleteAsync($"{methodToCall}/{invalidId}");

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}