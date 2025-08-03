global using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestingProject;

namespace TestingProject.OrderControlers
{
    [TestFixture]
    public class OrderController
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Setting up Order Controller tests");
        }
    }

    // Order_Create Class - Handles order creation actions
    [TestFixture]
    public class Order_Create : OrderController
    {
        private OrderService orderService;

        [SetUp]
        public void SetupOrderCreate()
        {
            orderService = new OrderService();
        }

        [Test]
        [Category("Smoke")]
        public void CreateOrder_ValidData_ShouldSucceed()
        {
            // Arrange
            var orderData = new CreateOrderRequest
            {
                UserId = Guid.NewGuid(),
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = Guid.NewGuid(), Quantity = 2, Price = 50.00m },
                    new OrderItem { ProductId = Guid.NewGuid(), Quantity = 1, Price = 25.00m }
                }
            };

            // Act
            var result = orderService.CreateOrder(orderData);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(125.00m, result.Order.TotalAmount);
            Assert.AreEqual(2, result.Order.Items.Count);
        }

        [Test]
        [Category("Smoke")]
        public void CreateOrder_EmptyItems_ShouldFail()
        {
            // Arrange
            var orderData = new CreateOrderRequest
            {
                UserId = Guid.NewGuid(),
                Items = new List<OrderItem>()
            };

            // Act
            var result = orderService.CreateOrder(orderData);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.Contains("Order must contain at least one item"));
        }

        [Test]
        [Category("Smoke")]
        public void CreateOrder_InvalidQuantity_ShouldFail()
        {
            // Arrange
            var orderData = new CreateOrderRequest
            {
                UserId = Guid.NewGuid(),
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = Guid.NewGuid(), Quantity = 0, Price = 50.00m }
                }
            };

            // Act
            var result = orderService.CreateOrder(orderData);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.Contains("Item quantity must be greater than zero"));
        }
    }
}