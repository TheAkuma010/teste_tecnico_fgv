namespace Sales.Domain.Entities
{
    public class OrderItem
    {
        public int OrderId { get; private set; }
        public int ProductId { get; private set; }
        public int Quantity { get; private set; } = 0;
        public decimal UnitPrice { get; private set; } = decimal.Zero;
        public decimal Total => UnitPrice * Quantity;

        private OrderItem()
        { }

        public OrderItem(int productId, int quantity, decimal unitPrice)
        {
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
        public void Update(int quantity, decimal unitPrice)
        {
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}