namespace Craftsmanship.Inventory
{
    public class ProductId
    {
        public ProductId(int value)
        {
            this.value = value;
        }

        public static implicit operator int(ProductId productId) => productId.value;
        public static implicit operator ProductId(int productId) => new ProductId(productId);

        private int value;
    }
}