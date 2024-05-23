using API.FurnitureStore.Data;
using API.FurnitureStore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.FurnitureStore.API.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly APIFurnitureStoreContext _context;

        public OrdersController(APIFurnitureStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> Get()
        {
            return await _context.Orders.Include(x => x.OrderDetails).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var order = await _context.Orders.Include(x => x.OrderDetails).FirstOrDefaultAsync(x => x.Id == id);

            if (order == null) return BadRequest();

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Order order)
        {
            if (order == null) return NotFound();
            if (order.OrderDetails == null) return BadRequest("You should have at least one detail");

            await _context.AddAsync(order);
            await _context.AddRangeAsync(order.OrderDetails);

            await _context.SaveChangesAsync();

            return CreatedAtAction("Post", order.Id, order);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Order order)
        {
            if (order == null) return NotFound();
            if (order.Id <= 0) return NotFound();

            var existingOrder = await _context.Orders.Include(x => x.OrderDetails).FirstOrDefaultAsync(x => x.Id == order.Id);
            if (existingOrder == null) return NotFound();

            existingOrder.OrderNumber = order.OrderNumber;
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.ClientId = order.ClientId;
            existingOrder.DeliveryDate = order.DeliveryDate;

            _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);

            _context.Orders.Update(existingOrder);
            await _context.OrderDetails.AddRangeAsync(order.OrderDetails);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Order order)
        {
            if (order == null) return NotFound();

            var existingOrder = await _context.Orders.Include(x => x.OrderDetails).FirstOrDefaultAsync(y => y.Id == order.Id); 
            if (existingOrder == null) return NotFound();

            _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);
            _context.Orders.Remove(existingOrder);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
