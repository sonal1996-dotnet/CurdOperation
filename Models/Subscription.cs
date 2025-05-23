namespace WebApp.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive => DateTime.Now >= StartDate && DateTime.Now <= EndDate;

        public ApplicationUser User { get; set; }
        public Plan Plan { get; set; }
    }

}
