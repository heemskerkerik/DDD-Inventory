using System.Collections.Generic;

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

            order.State.Should().Be(OrderState.Verified);

            domainEvents.RaisedEvents.ShouldAllBeEquivalentTo(new OrderReadyEvent(orderId: 234));
        }
    }

    internal class SpyDomainEvents: IDomainEvents
    {
        public IReadOnlyCollection<object> RaisedEvents => _raisedEvents;

        public void Raise<T>(T @event)
        {
            _raisedEvents.Add(@event);
        }

        private readonly List<object> _raisedEvents = new List<object>();
    }
}