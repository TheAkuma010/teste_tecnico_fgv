namespace Sales.Domain.Entities
{
    public class Order
    {
        private readonly List<OrderItem> _items = new();
        public int Id { get; private set; }
        public int ClientId { get; private set; }
        public string ClientName { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public decimal Total { get; private set; }
        public IReadOnlyCollection<OrderItem> OrderItems => _items.AsReadOnly();

        private Order()
        { }

        public Order(int clientId)
        {
            ClientId = clientId;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddItem(OrderItem item)
        {
            _items.Add(item);
            RecalculateTotal();
        }

        public void SetTotal(decimal total)
        {
            Total = total;
        }

        public void SetClientName(string clientName)
        {
            ClientName = clientName;
        }

        private void RecalculateTotal()
        {
            Total = _items.Sum(item => item.Total);
        }
    }
}
