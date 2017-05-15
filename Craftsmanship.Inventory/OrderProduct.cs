namespace Craftsmanship.Inventory
{
    public class OrderProduct
    {
        public OrderProduct(ProductId productId, ProductQuantity quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public ProductId ProductId { get; }
        public ProductQuantity Quantity { get; }
    }

    public enum OrderState
    {
        NotVerified,
        Verified,
        Rejected,
    }
}