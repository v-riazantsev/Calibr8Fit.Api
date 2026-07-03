using Calibr8Fit.Api.Interfaces.Model;
using Microsoft.AspNetCore.Identity;

namespace Calibr8Fit.Api.Models
{
    public class User : IdentityUser, IEntity<string>
    {
        public virtual UserProfile? Profile { get; set; }
        public virtual ICollection<UserActivity>? UserActivities { get; set; } = [];
        public virtual ICollection<UserFood>? UserFoods { get; set; } = [];
        public virtual ICollection<UserMeal>? UserMeals { get; set; } = [];
        public virtual ICollection<ActivityRecord>? ActivityRecords { get; set; } = [];
        public virtual ICollection<ConsumptionRecord>? ConsumptionRecords { get; set; } = [];
        public virtual ICollection<WaterIntakeRecord>? WaterIntakeRecords { get; set; } = [];
        public virtual ICollection<WeightRecord>? WeightRecords { get; set; } = [];
        public virtual ICollection<DailyBurnTarget>? DailyBurnTargets { get; set; } = [];
        public virtual ICollection<FriendRequest>? SentFriendRequests { get; set; } = [];
        public virtual ICollection<FriendRequest>? ReceivedFriendRequests { get; set; } = [];
        public virtual ICollection<User>? Friends { get; set; } = [];
        public virtual ICollection<UserFollower>? Followers { get; set; } = [];
        public virtual ICollection<UserFollower>? Following { get; set; } = [];
        public virtual ICollection<Post>? Posts { get; set; } = [];
        public virtual ICollection<Comment>? Comments { get; set; } = [];
        public virtual ICollection<ChatMember>? ChatMemberships { get; set; } = [];
        public virtual ICollection<ChatMessage>? ChatMessages { get; set; } = [];
        public virtual ICollection<ChatMessageRead>? ChatMessageReads { get; set; } = [];
    }
}