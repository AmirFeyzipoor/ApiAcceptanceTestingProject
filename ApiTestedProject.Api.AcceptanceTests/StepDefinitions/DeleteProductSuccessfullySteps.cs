using ApiTestedProject.Api.AcceptanceTests.Models;
using ApiTestedProject.Api.AcceptanceTests.Models.Clients;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Transactions;
using System.Xml.Linq;
using TechTalk.SpecFlow;

namespace ApiTestedProject.Api.AcceptanceTests.StepDefinitions
{
    [Binding]
    public class DeleteProductSuccessfullySteps
    {
        private readonly IProductClient _productClient;
        private int _deletedProductId;

        public DeleteProductSuccessfullySteps(IProductClient productClient)
        {
            _productClient = productClient;
        }

        [Given(@"There are two products with the names '([^']*)' and '([^']*)' and the prices of (.*) and (.*) already in the product list")]
        public async Task Given(string firstTestproduct, string secondTestproduct,
            int firstTestproductPrice, int secondTestproductPrice)
        {
            var firstDto = new AddProductDto()
            {
                Name = firstTestproduct,
                Price = firstTestproductPrice
            };
            _deletedProductId = await _productClient.Add(firstDto);
            var secondDto = new AddProductDto()
            {
                Name = secondTestproduct,
                Price = secondTestproductPrice
            };
            await _productClient.Add(secondDto);
        }

        [When(@"I will delete a product with '(.*)' name and (.*) price")]
        public async Task When(string firstTestproduct, int firstTestproductPrice)
        {
            await _productClient.Delete(_deletedProductId);
        }

        [Then(@"There should be one product with '(.*)' name and (.*) price in the product list")]
        public async Task Then(string secondTestproduct, int secondTestproductPrice)
        {
            var products = await _productClient.GetAll();
            products.Should().HaveCount(1);
            products!.FirstOrDefault()!.Name.Should().Be(secondTestproduct);
            products!.FirstOrDefault()!.Price.Should().Be(secondTestproductPrice);
        }
    }
}
