namespace Sales.Domain.Entities
{
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public decimal Price { get; private set; } = decimal.Zero;
        public int Stock { get; private set; } = 0;

        private Product()
        { }

        public Product(string name, decimal price, int stock)
        {
            Name = name;
            Price = price;
            Stock = stock;
        }

        public void Update(string name, decimal price, int stock)
        {
            Name = name;
            Price = price;
            Stock = stock;
        }

        public void DecreaseStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException(
                    "A quantidade deve ser maior que zero.");
            }

            if (Stock < quantity)
            {
                throw new Domain.Exceptions.DomainException(
                    $"Estoque insuficiente para o produto {Name}.");
            }

            Stock -= quantity;
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException(
                    "A quantidade deve ser maior que zero.");
            }

            Stock += quantity;
        }
    }
}
