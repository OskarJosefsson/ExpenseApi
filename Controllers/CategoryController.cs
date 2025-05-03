using ExpenseApi.Models;
using ExpenseApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepo _categoryRepo;

        public CategoryController(ICategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return BadRequest("Category data is null.");
            }

            try
            {
                var createdCategory = await _categoryRepo.CreateCategory(category);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryRepo.GetAllCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryRepo.GetCategoryById(id);

                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> UpdateCategory(int id, [FromBody] Category category)
        {
            if (category == null)
            {
                return BadRequest("Category data is null.");
            }

            try
            {
                var updatedCategory = await _categoryRepo.UpdateCategory(id, category);

                if (updatedCategory == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }

                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                var deleted = await _categoryRepo.DeleteCategory(id);

                if (!deleted)
                {
                    return NotFound($"Category with ID {id} not found.");
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
