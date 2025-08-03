global using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestingProject;

namespace TestingProject.ProductControlers
{
    [TestFixture]
    public class ProductController
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Setting up Product Controller tests");
        }
    }

    [TestFixture]
    public class Product_Create : ProductController
    {
        private ProductService productService;

        [SetUp]
        public void SetupProductCreate()
        {
            productService = new ProductService();
        }

        [Test]
        [Category("Smoke")]
        [TestCase("Laptop", "Electronics", 999.99)]
        [TestCase("Coffee Mug", "Kitchen", 15.50)]
        public void CreateProduct_ValidData_ShouldSucceed(string name, string category, decimal price)
        {
            // Arrange
            var productData = new CreateProductRequest
            {
                Name = name,
                Category = category,
                Price = price,
                Description = $"A great {name.ToLower()}"
            };

            // Act
            var result = productService.CreateProduct(productData);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.ProductId);
            Assert.AreEqual(name, result.Product.Name);
            Assert.AreEqual(price, result.Product.Price);
        }

        [Test]
        [Category("Smoke")]
        public void CreateProduct_NegativePrice_ShouldFail()
        {
            // Arrange
            var productData = new CreateProductRequest
            {
                Name = "Test Product",
                Category = "Test",
                Price = -10.00m,
                Description = "Test product"
            };

            // Act
            var result = productService.CreateProduct(productData);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.Contains("Price cannot be negative"));
        }

        [Test]
        [Category("Smoke")]
        public void CreateProduct_EmptyName_ShouldFail()
        {
            // Arrange
            var productData = new CreateProductRequest
            {
                Name = "",
                Category = "Test",
                Price = 10.00m,
                Description = "Test product"
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => productService.CreateProduct(productData));
        }
    }
}