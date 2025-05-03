using Microsoft.AspNetCore.Mvc;
using ExpenseApi.Models;
using ExpenseApi.Services;

namespace ExpenseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;

        public UserController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create([FromBody] User user)
        {
            if (user == null || user.UserId == Guid.Empty || string.IsNullOrEmpty(user.Name))
            {
                return BadRequest("User data is invalid.");
            }

            try
            {
                var createdUser = await _userRepo.CreateUser(user);
                return CreatedAtAction(nameof(GetById), new { id = createdUser.UserId }, createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(Guid id)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(id);

                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update(Guid id, [FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Name))
            {
                return BadRequest("Invalid user data.");
            }

            try
            {
                var updatedUser = await _userRepo.UpdateUser(id, user);

                if (updatedUser == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _userRepo.DeleteUser(id);

                if (!deleted)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
