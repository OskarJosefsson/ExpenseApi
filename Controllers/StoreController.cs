
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::ExpenseApi.Models;
    using global::ExpenseApi.Services;

    namespace ExpenseApi.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class StoreController : ControllerBase
        {
            private readonly IStoreService _storeService;

            public StoreController(IStoreService storeService)
            {
                _storeService = storeService;
            }


            [HttpGet]
            public async Task<IActionResult> GetStores()
            {
                var stores = await _storeService.GetStoresAsync();
                return Ok(stores);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetStore(int id)
            {
                var store = await _storeService.GetStoreAsync(id);

                if (store == null)
                {
                    return NotFound(); 
                }

                return Ok(store);
            }

            [HttpPost]
            public async Task<IActionResult> CreateStore([FromBody] Store store)
            {
                if (store == null)
                {
                    return BadRequest("Store data is required.");
                }

                var createdStore = await _storeService.CreateStoreAsync(store);
                return CreatedAtAction(nameof(GetStore), new { id = createdStore.Id }, createdStore);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateStore(int id, [FromBody] Store store)
            {
                if (store == null)
                {
                    return BadRequest("Store data is required.");
                }

                var result = await _storeService.UpdateStoreAsync(id, store);

                if (result)
                {
                    return Ok(store); 
                }

                return NotFound(); 
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteStore(int id)
            {
                var result = await _storeService.DeleteStoreAsync(id);

                if (result)
                {
                    return NoContent(); 
                }

                return NotFound();
            }

            [HttpPost("convert")]
            public IActionResult ConvertToStore([FromBody] string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Store name is required.");
                }

                var store = _storeService.ConvertToStore(name);

                return Ok(store); 
            }
        }
    }


