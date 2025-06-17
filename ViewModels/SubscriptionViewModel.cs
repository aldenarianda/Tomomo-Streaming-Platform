using Tomomo7.Models;

namespace Tomomo7.ViewModels
{
    public class SubscriptionViewModel
    {
        public List<SubscriptionPlan> AvailablePlans { get; set; } = new List<SubscriptionPlan>();

        public Subscription? CurrentSubscription { get; set; }
    }
}
