namespace Craftsmanship.Inventory
{
    public class ProductQuantity
    {
        public ProductQuantity(long value)
        {
            this.value = value;
        }

        public static implicit operator long(ProductQuantity quantity) => quantity.value;
        public static implicit operator ProductQuantity(long quantity) => new ProductQuantity(quantity);

        private long value;
    }
}