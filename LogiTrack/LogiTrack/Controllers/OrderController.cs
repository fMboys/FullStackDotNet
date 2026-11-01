using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace InventoryApi.Controllers
{
    [Authorize(Roles = "Manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        // In-memory data store for testing
        private static readonly List<Order> Orders = new List<Order>();

        // GET: api/orders
        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetOrders()
        {
            return Ok(Orders);
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public ActionResult<Order> GetOrder(int id)
        {
            var order = Orders.FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            return Ok(order);
        }

        // POST: api/orders
        [HttpPost]
        public ActionResult<Order> CreateOrder(Order order)
        {
            Orders.Add(order);

            return CreatedAtAction(
                nameof(GetOrder),
                new { id = order.OrderId },
                order);
        }

        // DELETE: api/orders/5
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var order = Orders.FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            Orders.Remove(order);

            return NoContent();
        }
    }
}