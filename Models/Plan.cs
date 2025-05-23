namespace WebApp.Models
{
    public class Plan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
    }

}
