using ExpenseApi.Models;
using ExpenseApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseRepo _expenseRepo;

        public ExpenseController(IExpenseRepo expenseRepo)
        {
            _expenseRepo = expenseRepo;
        }

        [HttpGet]
        public async Task<IEnumerable<Expense>> GetAll()
        {
            return await _expenseRepo.GetAllAsync();
        }

        [HttpGet("{id}")]
        public ActionResult<Expense> GetById(int id)
        {
            var expense = _expenseRepo.GetByIdAsync(id);
            if (expense == null) return NotFound();
            return Ok(expense);
        }


        [HttpPost]
        [HttpPost]
        public async Task<ActionResult<Expense>> Create([FromBody] Expense newExpense)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _expenseRepo.CreateAsync(newExpense);
                return CreatedAtAction(nameof(GetById), new { id = newExpense.Id }, newExpense);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Expense updatedExpense)
        {
            var existing = await _expenseRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Description = updatedExpense.Description;
            existing.Amount = updatedExpense.Amount;
            existing.Date = updatedExpense.Date;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _expenseRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _expenseRepo.DeleteAsync(id);
            return NoContent();
        }
    }

}
