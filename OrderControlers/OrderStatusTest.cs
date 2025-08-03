global using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestingProject;

namespace TestingProject.OrderControlers
{
    // Order_Status Class - Handles order status management actions
    [TestFixture]
    public class Order_Status : OrderController
    {
        private OrderStatusService statusService;

        [SetUp]
        public void SetupOrderStatus()
        {
            statusService = new OrderStatusService();
        }

        [Test]
        [Category("Regression")]
        [TestCase(OrderStatus.Pending, OrderStatus.Processing, true)]
        [TestCase(OrderStatus.Processing, OrderStatus.Shipped, true)]
        [TestCase(OrderStatus.Shipped, OrderStatus.Delivered, true)]
        [TestCase(OrderStatus.Delivered, OrderStatus.Pending, false)]
        public void UpdateOrderStatus_ValidTransitions_ShouldReturnExpectedResult(
            OrderStatus currentStatus, OrderStatus newStatus, bool shouldSucceed)
        {
            // Arrange
            var orderId = Guid.NewGuid();
            statusService.CreateTestOrder(orderId, currentStatus);

            // Act
            var result = statusService.UpdateOrderStatus(orderId, newStatus);

            // Assert
            Assert.AreEqual(shouldSucceed, result.Success);
            if (shouldSucceed)
            {
                var order = statusService.GetOrder(orderId);
                Assert.AreEqual(newStatus, order.Status);
            }
        }

        [Test]
        [Category("Regression")]
        public void GetOrderStatus_ValidOrderId_ShouldReturnStatus()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var expectedStatus = OrderStatus.Processing;
            statusService.CreateTestOrder(orderId, expectedStatus);

            // Act
            var status = statusService.GetOrderStatus(orderId);

            // Assert
            Assert.AreEqual(expectedStatus, status);
        }

        [Test]
        [Category("Regression")]
        public void UpdateOrderStatus_NonExistentOrder_ShouldFail()
        {
            // Arrange
            var nonExistentOrderId = Guid.NewGuid();

            // Act
            var result = statusService.UpdateOrderStatus(nonExistentOrderId, OrderStatus.Processing);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Order not found", result.ErrorMessage);
        }
    }
}