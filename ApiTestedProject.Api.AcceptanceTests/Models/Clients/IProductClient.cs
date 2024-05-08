using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTestedProject.Api.AcceptanceTests.Models.Clients
{
    public interface IProductClient
    {
        [Get("/products")]
        Task<List<Product>> GetAll();

        [Get("/products/{id}")]
        Task<Product?> Get(int id);

        [Post("/products")]
        Task<int> Add([Body]AddProductDto dto);

        [Delete("/products/{id}")]
        Task Delete(int id);
    }
}
