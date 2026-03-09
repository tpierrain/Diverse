namespace Diverse.Tests.Utils
{
    public class OrderWithNamedParameters
    {
        public string Description { get; }
        public string CustomerEmail { get; }
        public int Age { get; }
        public decimal Price { get; }

        public OrderWithNamedParameters(string description, string customerEmail, int age, decimal price)
        {
            Description = description;
            CustomerEmail = customerEmail;
            Age = age;
            Price = price;
        }
    }
}
