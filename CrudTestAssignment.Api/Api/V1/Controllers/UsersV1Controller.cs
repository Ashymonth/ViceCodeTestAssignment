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
            user.Id = await _repository.CreateAsync(user, cancellationToken);

            return Created(Url.RouteUrl("GetByName", new { userName = user.Name }), user);
        }

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

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var result = await _repository.GetAsync(cancellationToken);
            if (result != null)
                return Ok(result);

            return NoContent();
        }

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

            var user = await _repository.GetAsync(userId, cancellationToken);
            if (user == null)
            {
                _logger.LogError($"User with id {userId} not found");
                return NotFound($"User with id {userId} not found");
            }

            await _repository.UpdateAsync(user.Id, model.Name, cancellationToken);
            user.Name = model.Name;

            return Ok(user);
        }

        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int userId, CancellationToken cancellationToken)
        {
            var user = await _repository.GetAsync(userId, cancellationToken);
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