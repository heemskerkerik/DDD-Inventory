using System;
using System.Linq;

using FluentAssertions;

using Xunit;

namespace Craftsmanship.Inventory.Tests
{
    public class DomainTests
    {
        /// <summary>
        /// Given an order was created
        /// When payment is verified
        /// Then stock is decreased
        /// </summary>
        [Fact]
        public void OrderVerifiedRaisesOrderReadyEvent()
        {
            var product = new OrderProduct(productId: 123, quantity: 1);
            var domainEvents = new SpyDomainEvents();
            var order = new Order(orderId: 234, products: new[] { product })
                        {
                            DomainEvents =
                            {
                                Instance = domainEvents
                            }
                        };

            order.Verify();

            order.State.Should().Be(OrderState.Ready);

            domainEvents.RaisedEvents.ShouldAllBeEquivalentTo(new OrderReadyEvent(orderId: 234));
        }

        [Fact]
        public void OrderVerifiedAlreadyReadyDoesNotRaiseEvent()
        {
            var product = new OrderProduct(productId: 123, quantity: 1);
            var domainEvents = new SpyDomainEvents();
            var order = new Order(orderId: 234, products: new[] { product })
                        {
                            DomainEvents =
                            {
                                Instance = domainEvents,
                            }
                        };
            order.Verify();
            domainEvents.Clear();

            order.Verify();

            domainEvents.RaisedEvents.Should().BeEmpty();
            
            StockQuantity quantity = new StockQuantity(123);
            int intQuantity = quantity;
        }

        [Fact]
        public void StockDecreaseDecreasesStockQuantity()
        {
            var stock = new Stock(productId: 123, quantity: 4);

            stock.Decrease(amount: 1);

            stock.Quantity.Should().Be(new StockQuantity(3));
        }

        [Fact]
        public void StockDecreaseBelowZeroThrowsException()
        {
            var stock = new Stock(productId: 123, quantity: 4);

            stock.Invoking(obj => obj.Decrease(amount: 5))
                 .ShouldThrow<InsufficientStockException>();
        }

        [Fact]
        public void OrderReadyEventSingleProductDecreasesStock()
        {
            var product = new OrderProduct(productId: 123, quantity: 1);
            var order = new Order(orderId: 234, products: new[] { product });
            var orderRepository = new StubOrderRepository(order);

            var stock = new Stock(productId: 123, quantity: 2);
            var stockRepository = new SpyStockRepository(stock);

            var sut = new OrderReadyEventHandler(orderRepository, stockRepository);

            sut.Handle(new OrderReadyEvent(orderId: 234));

            stock.Quantity.Should().Be(new StockQuantity(1));
            stockRepository.SavedStock.Should().BeSameAs(stock);
        }

        private class StubOrderRepository: IOrderRepository
        {
            public Order Get(OrderId orderId)
            {
                if(orderId == order.OrderId)
                    return order;

                return null;
            }

            public StubOrderRepository(Order order)
            {
                this.order = order;
            }

            private readonly Order order;
        }

        private class SpyStockRepository: IStockRepository
        {
            public Stock Get(ProductId productId)
            {
                if(productId == stock.ProductId)
                    return stock;

                return null;
            }

            public void Save(Stock stock)
            {
                SavedStock = stock;
            }

            public Stock SavedStock { get; private set; }

            public SpyStockRepository(Stock stock)
            {
                this.stock = stock;
            }

            private readonly Stock stock;
        }
    }

    public interface IOrderRepository
    {
        Order Get(OrderId orderId);
    }

    public interface IStockRepository
    {
        Stock Get(ProductId productId);
        void Save(Stock stock);
    }

    public class OrderReadyEventHandler
    {
        public void Handle(OrderReadyEvent @event)
        {
            var order = orderRepository.Get(@event.OrderId);
            var product = order.Products.First();

            var stock = stockRepository.Get(product.ProductId);

            stock.Decrease(product.Quantity);

            stockRepository.Save(stock);
        }

        public OrderReadyEventHandler(IOrderRepository orderRepository, IStockRepository stockRepository)
        {
            this.orderRepository = orderRepository;
            this.stockRepository = stockRepository;
        }

        private readonly IOrderRepository orderRepository;
        private readonly IStockRepository stockRepository;
    }
}