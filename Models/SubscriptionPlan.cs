using System;
using System.Collections.Generic;

namespace Tomomo7.Models;

public partial class SubscriptionPlan
{
    public int PlanId { get; set; }

    public string PlanName { get; set; } = null!;

    public int DurationMonths { get; set; }

    public decimal Price { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
