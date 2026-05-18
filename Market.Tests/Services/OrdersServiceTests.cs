using FluentValidation;
using Market.Application.Models.Requests;
using Market.Application.Models.Responses; 
using Market.Application.Repositories;
using Market.Application.Services;
using Market.Application.Services.Abstractions;
using Market.Domain.Entities;
using Market.Domain.Enums;
using Market.Domain.ValueObjects; 
using Moq;
using AutoMapper;
using System.Reflection;

namespace Market.Tests.Services
{
    public class OrdersServiceTests
    {
        private readonly Mock<IOrdersRepository> _ordersRepoMock;
        private readonly Mock<IProductsRepository> _productsRepoMock;
        private readonly Mock<IUsersRepository> _usersRepoMock;
        private readonly Mock<IPricingEngine> _pricingEngineMock;
        private readonly Mock<ILoyaltyService> _loyaltyServiceMock;
        private readonly Mock<IInventoryService> _inventoryServiceMock; 
        private readonly Mock<IValidator<OrderCreateDto>> _createValidatorMock;
        private readonly Mock<IValidator<PagedRequestDto>> _pagedValidatorMock;
        private readonly Mock<IMapper> _mapperMock; 
        private readonly OrdersService _ordersService;

        public OrdersServiceTests()
        {
            _ordersRepoMock = new Mock<IOrdersRepository>();
            _productsRepoMock = new Mock<IProductsRepository>();
            _usersRepoMock = new Mock<IUsersRepository>();
            _pricingEngineMock = new Mock<IPricingEngine>();
            _loyaltyServiceMock = new Mock<ILoyaltyService>();
            _inventoryServiceMock = new Mock<IInventoryService>(); 
            _createValidatorMock = new Mock<IValidator<OrderCreateDto>>();
            _pagedValidatorMock = new Mock<IValidator<PagedRequestDto>>();
            _mapperMock = new Mock<IMapper>(); 

            _ordersService = new OrdersService(
                _ordersRepoMock.Object,
                _productsRepoMock.Object,
                _usersRepoMock.Object,
                _pricingEngineMock.Object,
                _loyaltyServiceMock.Object,
                _inventoryServiceMock.Object, 
                _createValidatorMock.Object,
                _pagedValidatorMock.Object,
                _mapperMock.Object); 
        }

        
        [Fact]
        public async Task Get_WhenOrderExists_CallsMapperAndReturnsDto()
        {
            long orderId = 1;
            long userId = 1;
            
            var order = CreateOrderWithId(orderId, userId);
            order.AddItem(1, Money.Create(500m, "UAH"), 1);
            
            var expectedDto = new OrderDto { Id = orderId, Price = 500m };

            _ordersRepoMock.Setup(x => x.Get(orderId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);
                
            _mapperMock.Setup(x => x.Map<OrderDto>(order))
                .Returns(expectedDto);

            var result = await _ordersService.Get(orderId, userId, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(500m, result.Price);
            
            _mapperMock.Verify(x => x.Map<OrderDto>(order), Times.Once); 
        }

        [Fact]
        public async Task Get_WhenOrderDoesNotExist_ThrowsInvalidOperationException()
        {
            long orderId = 1;
            long userId = 1;
            _ordersRepoMock.Setup(x => x.Get(orderId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _ordersService.Get(orderId, userId, CancellationToken.None));
            
            Assert.Contains("не знайдено", exception.Message);
        }

        

        [Fact]
        public async Task Ship_WhenStatusIsPending_ThrowsInvalidOperationException()
        {
            long orderId = 1;
            long userId = 1;
            
            var order = CreateOrderWithId(orderId, userId);

            _ordersRepoMock.Setup(x => x.Get(orderId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _ordersService.Ship(orderId, userId, CancellationToken.None));
            
            Assert.Contains("неможливо змінити статус", exception.Message);
        }

        [Fact]
        public async Task Deliver_WhenOrderIsValid_UpdatesLoyalty()
        {
            long orderId = 1;
            long userId = 1;
            
            var order = CreateOrderWithId(orderId, userId);
            
            typeof(Order).GetProperty("Status")!.SetValue(order, OrderStatus.Shipped);

            _ordersRepoMock.Setup(x => x.Get(orderId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            await _ordersService.Deliver(orderId, userId, CancellationToken.None);

            Assert.Equal(OrderStatus.Delivered, order.Status);
            _ordersRepoMock.Verify(x => x.Update(order, It.IsAny<CancellationToken>()), Times.Once);
            
            _loyaltyServiceMock.Verify(x => x.UpdateUserLoyaltyAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Cancel_WhenOrderIsValid_CallsInventoryRelease()
        {
            long orderId = 1;
            long userId = 1;
            
            var order = CreateOrderWithId(orderId, userId);
            order.AddItem(1, Money.Create(100m, "UAH"), 2);

            _ordersRepoMock.Setup(x => x.Get(orderId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            await _ordersService.Cancel(orderId, userId, CancellationToken.None);

            Assert.Equal(OrderStatus.Cancelled, order.Status);
            _ordersRepoMock.Verify(x => x.Update(order, It.IsAny<CancellationToken>()), Times.Once);
            
            _inventoryServiceMock.Verify(x => x.ReleaseReservedProductsAsync(order.Items, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(OrderStatus.Pending, OrderStatus.Confirmed, true)]
        [InlineData(OrderStatus.Pending, OrderStatus.Cancelled, true)]
        [InlineData(OrderStatus.Confirmed, OrderStatus.Shipped, true)]
        [InlineData(OrderStatus.Confirmed, OrderStatus.Cancelled, true)]
        [InlineData(OrderStatus.Shipped, OrderStatus.Delivered, true)]
        [InlineData(OrderStatus.Pending, OrderStatus.Shipped, false)]
        [InlineData(OrderStatus.Pending, OrderStatus.Delivered, false)]
        [InlineData(OrderStatus.Confirmed, OrderStatus.Delivered, false)]
        [InlineData(OrderStatus.Shipped, OrderStatus.Cancelled, false)]
        [InlineData(OrderStatus.Delivered, OrderStatus.Cancelled, false)]
        [InlineData(OrderStatus.Cancelled, OrderStatus.Confirmed, false)]
        [InlineData(OrderStatus.Cancelled, OrderStatus.Shipped, false)]
        public async Task OrderStatusTransitions_ShouldBehaveCorrectly(
            OrderStatus currentStatus, 
            OrderStatus targetStatus, 
            bool shouldSucceed)
        {
            long orderId = 1;
            long userId = 1;

            var order = CreateOrderWithId(orderId, userId);
            order.AddItem(1, Money.Create(100m, "UAH"), 1);

            typeof(Order).GetProperty("Status")!.SetValue(order, currentStatus);

            _ordersRepoMock.Setup(x => x.Get(orderId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            Func<Task> act = targetStatus switch
            {
                OrderStatus.Confirmed => () => _ordersService.Confirm(orderId, userId, CancellationToken.None),
                OrderStatus.Shipped => () => _ordersService.Ship(orderId, userId, CancellationToken.None),
                OrderStatus.Delivered => () => _ordersService.Deliver(orderId, userId, CancellationToken.None),
                OrderStatus.Cancelled => () => _ordersService.Cancel(orderId, userId, CancellationToken.None),
                _ => throw new ArgumentException($"Тестування переходу в статус {targetStatus} не підтримується")
            };

            if (shouldSucceed)
            {
                await act.Invoke();
                Assert.Equal(targetStatus, order.Status);
            }
            else
            {
                var exception = await Assert.ThrowsAsync<InvalidOperationException>(act);
                Assert.Contains("неможливо змінити статус", exception.Message);
                Assert.Equal(currentStatus, order.Status);
            }
        }

        private static Order CreateOrderWithId(long id, long userId)
        {
            var order = Order.Create(userId);
            
            var propertyInfo = typeof(Order).GetProperty("Id");
            propertyInfo?.SetValue(order, id);
            
            return order;
        }
    }
}