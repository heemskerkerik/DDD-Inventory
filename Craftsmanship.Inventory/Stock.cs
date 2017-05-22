using System;

namespace Craftsmanship.Inventory
{
    public class Stock
    {
        public ProductId ProductId { get; }
        public StockQuantity Quantity { get; private set; }

        public void Decrease(StockQuantity amount)
        {
            if(Quantity - amount < 0)
                throw new InsufficientStockException();
            
            Quantity -= amount;
        }

        public Stock(ProductId productId, StockQuantity quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
    
    public class InsufficientStockException: Exception
    {
    }

    public class StockQuantity
    {
        public StockQuantity(int value)
        {
            this.value = value;
        }

        protected bool Equals(StockQuantity other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
                return false;
            if(ReferenceEquals(this, obj))
                return true;

            if(obj is StockQuantity other)
                return Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            return value;
        }

        public static bool operator ==(StockQuantity left, StockQuantity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StockQuantity left, StockQuantity right)
        {
            return !Equals(left, right);
        }

        public static StockQuantity operator -(StockQuantity left, StockQuantity right)
        {
            return new StockQuantity(left.value - right.value);
        }

        public static implicit operator int(StockQuantity orderId) => orderId.value;
        public static implicit operator StockQuantity(int orderId) => new StockQuantity(orderId);
        public static implicit operator StockQuantity(ProductQuantity quantity) => new StockQuantity((int) quantity);

        public override string ToString() => value.ToString();

        private readonly int value;
    }
}