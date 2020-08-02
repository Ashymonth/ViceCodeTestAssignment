using CrudTestAssignment.Api.Api.V1.Models;
using CrudTestAssignment.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<UsersV1Controller> _logger;

        public UsersV1Controller(IRepository repository, ILogger<UsersV1Controller> logger)
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
        /// <response code="409">If user with this name already exist</response>
        /// <response code="500">Sql exception</response>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async ValueTask<IActionResult> CreateUser([FromBody] UserModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError(ErrorMessages.ModelIsNotValid);
                
                return BadRequest(ErrorMessages.ModelIsNotValid);
            }

            var existUser = await _repository.GetByNameAsync(model.Name, cancellationToken);
            if (existUser != null)
            {
                _logger.LogError(string.Format(ErrorMessages.UserWithThisNameExistPlaceHolder,model.Name));

                return Conflict(string.Format(ErrorMessages.UserWithThisNameExistPlaceHolder,model.Name));
            }

            var user = Mapper.Mapper.MapToEntity(model);

            user = await _repository.CreateAsync(user, cancellationToken);

            return Created(Url.RouteUrl("GetByName", new { userName = user.Name }), Mapper.Mapper.MapToModel(user));
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
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> GetUserByNameAsync(string userName, CancellationToken cancellationToken)
        {
            var result = await _repository.GetByNameAsync(userName, cancellationToken);
            if (result != null)
                return Ok(Mapper.Mapper.MapToModel(result));

            _logger.LogError(string.Format(ErrorMessages.UserNameNotFoundPlaceHolder,userName));

            return NotFound(string.Format(ErrorMessages.UserNameNotFoundPlaceHolder,userName));
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
        [ProducesResponseType(typeof(IEnumerable<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllAsync(cancellationToken);
            if (result != null)
                return Ok(Mapper.Mapper.MapToModels(result));

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
        /// <response code="409">If user with this name already exist</response>
        /// <response code="500">Sql exception</response>
        /// <returns></returns>
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async ValueTask<IActionResult> UpdateUser(int userId, [FromBody] UserModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError(ErrorMessages.ModelIsNotValid);

                return BadRequest(ErrorMessages.ModelIsNotValid);
            }

            var user = await _repository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                _logger.LogError(string.Format(ErrorMessages.UserIdNotFoundPlaceHolder, userId));
                return NotFound(string.Format(ErrorMessages.UserIdNotFoundPlaceHolder, userId));
            }

            var existUser = await _repository.GetByNameAsync(model.Name, cancellationToken);
            if (existUser != null)
            {
                _logger.LogError(ErrorMessages.UserWithThisNameExistPlaceHolder);
             
                return Conflict(ErrorMessages.UserWithThisNameExistPlaceHolder);
            }

            await _repository.UpdateAsync(user.Id, model.Name, cancellationToken);
            user.Name = model.Name;

            return Ok(Mapper.Mapper.MapToModel(user));
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
        public async ValueTask<IActionResult> DeleteUser(int userId, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                _logger.LogError(string.Format(ErrorMessages.UserIdNotFoundPlaceHolder,userId));
                
                return NotFound();
            }

            await _repository.DeleteAsync(user.Id, cancellationToken);

            return NoContent();
        }
    }
}