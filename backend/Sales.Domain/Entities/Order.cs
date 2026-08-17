namespace Sales.Domain.Entities
{
    public class Order
    {
        private readonly List<OrderItem> _items = new();
        public int Id { get; private set; }
        public int ClientId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public decimal Total => _items.Sum(item => item.Total);
        public IReadOnlyCollection<OrderItem> OrderItems => _items.AsReadOnly();

        private Order()
        { }

        public Order(int clientId)
        {
            ClientId = clientId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}