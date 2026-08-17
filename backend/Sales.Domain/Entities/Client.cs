namespace Sales.Domain.Entities
{
    public class Client
    {
        public int Id { get; private set; }
        public string Cnpj { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        private Client()
        { }

        public Client(string cnpj, string name, string email)
        {
            Cnpj = cnpj;
            Name = name;
            Email = email;
            CreatedAt = DateTime.UtcNow;
        }
    }
}