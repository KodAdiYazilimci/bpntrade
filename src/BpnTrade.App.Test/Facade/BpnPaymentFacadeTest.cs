using BpnTrade.App.Facades;
using BpnTrade.App.Persistence;
using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;
using BpnTrade.Domain.Dto.Payment;
using BpnTrade.Domain.Entities;
using BpnTrade.Domain.Persistence;
using BpnTrade.Domain.Roots;

using Microsoft.EntityFrameworkCore;

using Moq;

using System.Collections.ObjectModel;

namespace BpnTrade.App.Test.Facade
{
    [TestClass]
    public class BpnPaymentFacadeTest
    {
        private Mock<IPreOrderAdapter> preOrderAdapterMock;
        private Mock<ICompleteAdapter> completeAdapterMock;
        private Mock<ICancelAdapter> cancelAdapterMock;
        private Mock<IProductAdapter> productAdapterMock;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private BpnContext context;

        [TestInitialize]
        public void Init()
        {
            preOrderAdapterMock = new Mock<IPreOrderAdapter>();
            completeAdapterMock = new Mock<ICompleteAdapter>();
            cancelAdapterMock = new Mock<ICancelAdapter>();
            productAdapterMock = new Mock<IProductAdapter>();
            unitOfWorkMock = new Mock<IUnitOfWork>();

            context = new BpnContext(new DbContextOptionsBuilder<BpnContext>()
                .UseInMemoryDatabase("MockDb").Options);
        }

        [TestMethod]
        public async Task When_Balance_Couldnt_Fetched()
        {
            var productAdapterMock = new Mock<IProductAdapter>();
            var balanceAdapterMock = new Mock<IBalanceAdapter>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            productAdapterMock
                .Setup(x => x.GetProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultRoot.Success(new ProductResponseDto()
                {
                    Success = true,
                    Data = new List<ProductResponseData>()
                    {
                        new ProductResponseData(){ Id = "prd-1", Price = 10, Stock = 5 }
                    }
                }));

            var order = new OrderEntity();
            order.UserId = "1";

            var orderItem = new OrderItemEntity();
            orderItem.Quantity = 1;
            orderItem.UnitPrice = 10;
            orderItem.Quantity = 2;
            orderItem.ProductId = "prd-1";
            orderItem.Order = order;

            order.OrderItems = new Collection<OrderItemEntity>();
            order.OrderItems.Add(orderItem);
            context.Orders.Add(order);
            context.OrderItems.Add(orderItem);

            await context.SaveChangesAsync();

            unitOfWorkMock.SetupGet(x => x.Context).Returns(context);

            balanceAdapterMock
                .Setup(x => x.GetUserBalanceAsync(It.IsAny<BalanceRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultRoot.Failure<BalanceResponseDto>(new ErrorDto("BLC002", "Balance info couldnt fetch")));

            var paymentFacade = new BpnPaymentFacade(
                balanceAdapter: balanceAdapterMock.Object,
                preOrderAdapter: preOrderAdapterMock.Object,
                completeAdapter: completeAdapterMock.Object,
                cancelAdapter: cancelAdapterMock.Object,
                productAdapter: productAdapterMock.Object,
                unitOfWork: unitOfWorkMock.Object);

            var paymentResult = await paymentFacade.ProcessPayment(new ProcessPaymentRequestDto()
            {
                Amount = 100,
                OrderId = 1
            });

            Assert.IsTrue(paymentResult?.Error?.Code == "BLC002");
        }

        [TestMethod]
        public async Task When_Balance_Is_Insufficient()
        {
            var balanceAdapterMock = new Mock<IBalanceAdapter>();

            balanceAdapterMock
                .Setup(x => x.GetUserBalanceAsync(It.IsAny<BalanceRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ResultRoot.Success(new BalanceResponseDto()
                    {
                        Success = true,
                        Data = new BalanceResponseData()
                        {
                            AvailableBalance = 0
                        }
                    }));


            var paymentFacade = new BpnPaymentFacade(
                balanceAdapter: balanceAdapterMock.Object,
                preOrderAdapter: preOrderAdapterMock.Object,
                completeAdapter: completeAdapterMock.Object,
                cancelAdapter: cancelAdapterMock.Object,
                productAdapter: productAdapterMock.Object,
                unitOfWork: unitOfWorkMock.Object);

            var paymentResult = await paymentFacade.ProcessPayment(new ProcessPaymentRequestDto()
            {
                Amount = 100,
                OrderId = 1
            });

            Assert.IsTrue(paymentResult?.Error?.Code == "BLC001");
        }
    }
}
