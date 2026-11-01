using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LogiTrack.Controllers
{
    [Authorize(Roles = "Manager")]
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly LogiTrackContext _context;
        private readonly IMemoryCache _cache;

        // Dependency Injection

        public InventoryController(LogiTrackContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: /api/inventory
    [HttpGet]
    public async Task<IActionResult> GetInventory()
    {
        const string cacheKey = "inventory_list";

        if (!_cache.TryGetValue(cacheKey, out List<InventoryItem> inventory))
        {
            // Retrieve from database
            inventory = _context.InventoryItems.ToList();

            // Cache for 30 seconds
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

            _cache.Set(cacheKey, inventory, cacheOptions);
        }

        return Ok(inventory);
    }

        // POST: /api/inventory
        [HttpPost]
        public ActionResult<InventoryItem> AddInventoryItem([FromBody] InventoryItem item)
        {
            if (item == null)
            {
                return BadRequest("Invalid inventory item.");
            }

            _context.InventoryItems.Add(item);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetInventory),
                new { id = item.ItemId },
                item);
        }

        // DELETE: /api/inventory/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteInventoryItem(int id)
        {
            var item = _context.InventoryItems.FirstOrDefault(i => i.ItemId == id);

            if (item == null)
            {
                return NotFound($"Item with ID {id} was not found.");
            }

            _context.InventoryItems.Remove(item);
            _context.SaveChanges();

            return NoContent();
        }
    }
}