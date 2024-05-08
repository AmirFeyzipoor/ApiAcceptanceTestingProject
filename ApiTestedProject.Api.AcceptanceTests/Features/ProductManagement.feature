Feature: Manage products in the system

Scenario: Product created successfully
	Given There are no products in the product list
	When I will create a product with 'test product' name and 200000 price
	Then There should be only one product with 'test product' name and 200000 price in product list

Scenario: Product deleted successfully
	Given There are two products with the names 'firstTestproduct' and 'secondTestproduct' and the prices of 2000 and 3000 already in the product list
	When I will delete a product with 'firstTestproduct' name and 2000 price
	Then There should be one product with 'secondTestproduct' name and 3000 price in the product list
