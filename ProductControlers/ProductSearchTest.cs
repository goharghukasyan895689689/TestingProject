global using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestingProject;

namespace TestingProject.ProductControlers
{
    [TestFixture]
    public class Product_Search : ProductController
    {
        private ProductSearchService searchService;

        [SetUp]
        public void SetupProductSearch()
        {
            searchService = new ProductSearchService();
            // Create some test products
            searchService.AddTestProducts();
        }

        [Test]
        [Category("Regression")]
        [TestCase("laptop", 2)]
        [TestCase("coffee", 1)]
        [TestCase("nonexistent", 0)]
        public void SearchProducts_ByName_ShouldReturnExpectedCount(string searchTerm, int expectedCount)
        {
            // Act
            var results = searchService.SearchByName(searchTerm);

            // Assert
            Assert.AreEqual(expectedCount, results.Count);
        }

        [Test]
        [Category("Regression")]
        public void SearchProducts_ByPriceRange_ShouldReturnProductsInRange()
        {
            // Arrange
            var minPrice = 100m;
            var maxPrice = 1000m;

            // Act
            var results = searchService.SearchByPriceRange(minPrice, maxPrice);

            // Assert
            Assert.IsTrue(results.All(p => p.Price >= minPrice && p.Price <= maxPrice));
        }

        [Test]
        [Category("Regression")]
        public void SearchProducts_ByCategory_ShouldReturnOnlyProductsInCategory()
        {
            // Arrange
            var category = "Electronics";

            // Act
            var results = searchService.SearchByCategory(category);

            // Assert
            Assert.IsTrue(results.All(p => p.Category == category));
        }
    }
}