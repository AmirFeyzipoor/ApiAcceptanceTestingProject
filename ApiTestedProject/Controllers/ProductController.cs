using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApiTestedProject.Infrastructor;
using WebApiTestedProject.Models;
using WebApiTestedProject.Models.Dtos;

namespace ApiTestedProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly OnlineShopDbContext _context;
        private readonly ILogger _logger;

        public ProductsController(OnlineShopDbContext context, 
            ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<Product>> GetAll()
        {
            Console.WriteLine("call GetAll products successfully");
            return await _context.Set<Product>().ToListAsync();
        }

        [HttpGet("cintainertest")]
        public async Task<List<Product>> Get()
        {
            return await _context.Set<Product>().ToListAsync();
        }

        [HttpPost]
        public async Task<int> Add(AddProductDto dto)
        {
            var product = new Product(dto.Name, dto.Price);

            await _context.AddAsync(product);

            await _context.SaveChangesAsync();

            return product.Id;
        }

        [HttpGet("{id}")]
        public async Task<Product?> Get(int id)
        {
            return await _context.Set<Product>().FirstOrDefaultAsync(_ => _.Id == id);
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var product = await _context.Set<Product>().FirstOrDefaultAsync(_ => _.Id == id);

            if (product != null)
            {
                _context.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
