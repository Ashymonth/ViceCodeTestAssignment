using CrudTestAssignment.DAL;
using CrudTestAssignment.DAL.Models;
using CrudTestAssignment.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CrudTestAssignment.Api.Api.V1.Models;

namespace CrudTestAssignment.Api.Api.V1.Controllers
{
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("api/v1/users")]
    public class UsersV1Controller : ControllerBase
    {
        private readonly IRepository _repository;

        private readonly ILoggerManager _logger;

        public UsersV1Controller(IRepository repository, ILoggerManager logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Adds user to the database and send his to client.
        /// </summary>
        /// <remarks>
        /// Get/api/v1/users/createUser{"name" : "example"}
        /// </remarks>
        /// <param name="model">A user to add view model</param>
        /// <param name="cancellationToken">Cancel operation</param>
        /// <response code="201">If user name not null or whitespace</response>
        /// <response code="400">If user name null or whitespace</response>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] UserViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Model was invalid");
                return BadRequest("User name is required");
            }

            var user = new User
            {
                CreatedDate = DateTime.Now,
                Name = model.Name

            };
            user = await _repository.CreateAsync(user, cancellationToken);

            return Created(Url.RouteUrl("GetByName", new { userName = user.Name }), user);
        }

        /// <summary>
        /// Returns the user from the database.
        /// </summary>
        /// <remarks>
        /// Get/api/v1/users/userId
        /// </remarks>
        /// <param name="userName">A user to return</param>
        /// <param name="cancellationToken">Cancel operation</param>
        /// <response code="200">If the user was found</response>
        /// <response code="404">If the user was not found</response>
        /// <returns></returns>
        [HttpGet("{userName}", Name = "GetByName")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserByNameAsync(string userName, CancellationToken cancellationToken)
        {
            var result = await _repository.GetByNameAsync(userName, cancellationToken);
            if (result != null)
                return Ok(result);

            _logger.LogError($"User with with name {userName} not found");

            return NotFound($"User with with name {userName} not found");
        }

        /// <summary>
        /// Returns all users from the database.
        /// </summary>
        /// <remarks>
        /// Get/api/v1/users/all
        /// </remarks>
        /// <param name="cancellationToken">Cancel operation</param>
        /// <response code="200">If the database is not empty</response>
        /// <response code="404">If the database is empty</response>

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllAsync(cancellationToken);
            if (result != null)
                return Ok(result);

            return NoContent();
        }

        /// <summary>
        /// Update the user in the database.
        /// </summary>
        /// <remarks>
        /// Put/api/v1/users/userId
        /// </remarks>
        /// <param name="model">User view model</param>
        /// <param name="cancellationToken">Cancel operation</param>
        /// <param name="userId">User id</param>
        /// <response code="200">If the user was found and updated</response>
        /// <response code="400">If the user model was invalid</response>
        /// <response code="404">If the user was not not found or not updated</response>
        /// <returns></returns>
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Model was not valid");
                return BadRequest("Model was not valid");
            }

            var user = await _repository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                _logger.LogError($"User with id {userId} not found");
                return NotFound($"User with id {userId} not found");
            }

            await _repository.UpdateAsync(user.Id, model.Name, cancellationToken);
            user.Name = model.Name;

            return Ok(user);
        }

        /// <summary>
        /// Delete the user in the database.
        /// </summary>
        /// <remarks>
        /// Delete/api/v1/users/userId
        /// </remarks>
        /// <param name="userId">User id</param>
        /// <param name="cancellationToken">Cancel operation</param>
        /// <response code="204">If the user was found and deleted</response>
        /// <response code="404">If the user was not not found</response>
        /// <returns></returns>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int userId, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                _logger.LogError($"User with id {userId} not found");
                return NotFound("User not found");
            }

            await _repository.DeleteAsync(user.Id, cancellationToken);

            return NoContent();
        }
    }
}