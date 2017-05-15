using System.Collections.Generic;

using AmbientContext;

namespace Craftsmanship.Inventory
{
    public class Order
    {
        public AmbientDomainEventsService DomainEvents = new AmbientDomainEventsService();

        public OrderId OrderId { get; }
        public IReadOnlyCollection<OrderProduct> Products { get; }
        public OrderState State { get; private set; }

        public Order(OrderId orderId, IReadOnlyCollection<OrderProduct> products)
        {
            OrderId = orderId;
            Products = products;
            State = OrderState.NotVerified;
        }

        public void VerifyPayment()
        {
            State = OrderState.Verified;
            DomainEvents.Raise(new OrderPaidEvent(OrderId));
        }
    }

    public class AmbientDomainEventsService: AmbientService<IDomainEvents>, IDomainEvents
    {
        public void Raise<T>(T @event)
        {
            Instance.Raise(@event);
        }
    }

    public interface IDomainEvents
    {
        void Raise<T>(T @event);
    }

    public class OrderPaidEvent
    {
        public OrderPaidEvent(OrderId orderId)
        {
            OrderId = orderId;
        }

        public OrderId OrderId { get; }
    }

    public class OrderId
    {
        public OrderId(int value)
        {
            this.value = value;
        }

        protected bool Equals(OrderId other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
                return false;
            if(ReferenceEquals(this, obj))
                return true;
            if(obj.GetType() != GetType())
                return false;
            return Equals((OrderId) obj);
        }

        public override int GetHashCode()
        {
            return value;
        }

        public static bool operator ==(OrderId left, OrderId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OrderId left, OrderId right)
        {
            return !Equals(left, right);
        }

        public static implicit operator int(OrderId orderId) => orderId.value;
        public static implicit operator OrderId(int orderId) => new OrderId(orderId);

        public override string ToString() => value.ToString();

        private readonly int value;
    }
}