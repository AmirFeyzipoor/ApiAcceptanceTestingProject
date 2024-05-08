using ApiTestedProject.Api.AcceptanceTests.Hooks;
using ApiTestedProject.Api.AcceptanceTests.Models;
using ApiTestedProject.Api.AcceptanceTests.Models.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ApiTestedProject.Api.AcceptanceTests.StepDefinitions
{
    [Binding]
    public class AddProductSuccessfullySteps
    {
        private readonly IProductClient _productClient;
        private int? _productId;

        public AddProductSuccessfullySteps(
            IProductClient productClient)
        {
            _productClient = productClient;
        }

        [Given(@"There are no products in the product list")]
        public void Given()
        {
        }

        [When(@"I will create a product with '(.*)' name and (.*) price")]
        public async Task When(string name, int price)
        {
            var dto = new AddProductDto()
            {
                Name = name,
                Price = price
            };
            _productId = await _productClient.Add(dto);
        }

        [Then(@"There should be only one product with '(.*)' name and (.*) price in product list")]
        public async Task Then(string name, int price)
        {
            _productId.Should().NotBe(null);

            var products = await _productClient.GetAll();
            products.Should().HaveCount(1);
            products!.FirstOrDefault()!.Name.Should().Be(name);
            products!.FirstOrDefault()!.Price.Should().Be(price);

        }
    }
}
