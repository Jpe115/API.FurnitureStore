﻿using API.FurnitureStore.Data;
using API.FurnitureStore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.FurnitureStore.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly APIFurnitureStoreContext _context;

        public ProductCategoriesController(APIFurnitureStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<ProductCategory>> Get()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var productCategory = await _context.ProductCategories.FirstOrDefaultAsync(c => c.Id == id);
            if (productCategory == null) return NotFound();

            return Ok(productCategory);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductCategory productCategory)
        {
            await _context.ProductCategories.AddAsync(productCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Post), productCategory.Id, productCategory);
        }

        [HttpPut]
        public async Task<IActionResult> Put(ProductCategory productCategory)
        {
            var searchedProductCategory = await _context.ProductCategories.FindAsync(productCategory.Id);
            if (searchedProductCategory == null) return NotFound();

            searchedProductCategory.Name = productCategory.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(ProductCategory productCategory)
        {
            if (productCategory == null) return NotFound();

            _context.ProductCategories.Remove(productCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
