using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
namespace Project123.Services
{

    public class Order
    {
        public int Id { get; set; }
        public string? Status { get; set; }
    }
    public interface IOrderRepository
    {
        public Order? GetOrderById(int id);
    }
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public string GetOrderStatus(int orderId)
        {
            var order = _orderRepository.GetOrderById(orderId);
            return order?.Status ?? "Order not found";
        }
    }

    public class OrderServiceTests
    {
        [Fact]
        public void GetOrderStatus_ShouldReturnStatus_WhenOrderExists()
        {
            // Arrange
            var mockOrderRepository = new Mock<IOrderRepository>();
               mockOrderRepository.Setup(repo => repo.GetOrderById(It.IsAny<int>())).Returns(new Order { Id = 1, Status = "Shipped" });
            //    mockOrderRepository.Setup(repo => repo.GetOrderById(It.IsAny<int>())).Returns((Order)null);
            // mockOrderRepository.Setup(repo => repo.GetOrderById(It.IsAny<int>())).Throws(new Exception("Repository error"));
            //   mockOrderRepository.Setup(repo => repo.GetOrderById(It.IsAny<int>())).Returns(new Order { Id = 1, Status = "Invalid" });
              // mockOrderRepository.Setup(repo => repo.GetOrderById(It.IsAny<int>())).Returns(new Order { Id = 1, Status = "Error" });
           // mockOrderRepository.Setup(repo => repo.GetOrderById(It.IsAny<int>())).Throws(new ArgumentException("Invalid order ID"));
            var orderService = new OrderService(mockOrderRepository.Object);

            // Act
            var result = orderService.GetOrderStatus(1);

            // Assert
            Assert.Equal("Shipped", result);
        }

        [Fact]
        public void GetOrderStatus_ShouldReturnNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var mockOrderRepository = new Mock<IOrderRepository>();
          mockOrderRepository.Setup(repo => repo.GetOrderById(It.IsAny<int>())).Returns((Order?)null);

            var orderService = new OrderService(mockOrderRepository.Object);

            // Act
            var result = orderService.GetOrderStatus(1);

            // Assert
            Assert.Equal("Order not found", result);
        }

        //[Fact]
        //public void testXTest()
        //{
        //    var order = new Order { Id = 1, Status = "Error" };

        //    Assert.Equal((Order)order, order);
        //}
        
    }
    }
