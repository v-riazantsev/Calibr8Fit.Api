using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Models.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Calibr8Fit.Api.Data
{
    public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
    {
        public required DbSet<DataVersion> DataVersions { get; set; }
        public required DbSet<UserProfile> UserProfiles { get; set; }
        public required DbSet<RefreshToken> RefreshTokens { get; set; }
        public required DbSet<PushToken> PushTokens { get; set; }
        public required DbSet<ActivityBase> BaseActivities { get; set; }
        public required DbSet<Food> BaseFood { get; set; }
        public required DbSet<ActivityRecord> ActivityRecords { get; set; }
        public required DbSet<ConsumptionRecord> ConsumptionRecords { get; set; }
        public required DbSet<WaterIntakeRecord> WaterIntakeRecords { get; set; }
        public required DbSet<WeightRecord> WeightRecords { get; set; }
        public IQueryable<Activity> Activities => Set<ActivityBase>().OfType<Activity>();
        public IQueryable<UserActivity> UserActivities => Set<ActivityBase>().OfType<UserActivity>();
        public IQueryable<Food> Foods => Set<FoodBase>().OfType<Food>();
        public IQueryable<UserFood> UserFoods => Set<FoodBase>().OfType<UserFood>();
        public required DbSet<UserMeal> UserMeals { get; set; }
        public required DbSet<UserMealItem> UserMealItems { get; set; }
        public required DbSet<DailyBurnTarget> DailyBurnTargets { get; set; }
        public required DbSet<FriendRequest> FriendRequests { get; set; }
        public required DbSet<Friendship> Friendships { get; set; }
        public required DbSet<UserFollower> UserFollowers { get; set; }
        public required DbSet<ProfilePicture> ProfilePictures { get; set; }
        public required DbSet<Post> Posts { get; set; }
        public required DbSet<Comment> Comments { get; set; }
        public required DbSet<PostLike> PostLikes { get; set; }
        public required DbSet<PostImage> PostImages { get; set; }
        public required DbSet<Chat> Chats { get; set; }
        public required DbSet<ChatMember> ChatMembers { get; set; }
        public required DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Create roles
            var roles = new List<IdentityRole>{
                new() {
                    Id = "Admin",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new() {
                    Id = "User",
                    Name = "User",
                    NormalizedName = "USER"
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);

            // Configure Data Versions
            builder.Entity<DataVersion>()
                .HasKey(dv => dv.DataResource); // DataResource is the primary key in DataVersion

            // Configure User Profile
            builder.Entity<UserProfile>()
                .HasOne<User>()
                .WithOne(u => u.Profile) // User has one UserProfile
                .HasForeignKey<UserProfile>(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> UserProfile

            // Configure ProfilePicture
            builder.Entity<ProfilePicture>()
                .HasKey(pp => new { pp.UserId, pp.FileName }); // Composite key

            builder.Entity<ProfilePicture>()
                .HasOne<UserProfile>()
                .WithMany(up => up.ProfilePictures) // UserProfiles can have many ProfilePictures
                .HasForeignKey(pp => pp.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for UserProfile -> ProfilePicture

            builder.Entity<ProfilePicture>()
                .HasIndex(pp => pp.UserId); // Index on UserId for efficient lookups

            // Configure RefreshToken
            builder.Entity<RefreshToken>()
                .HasKey(rt => new { rt.UserId, rt.DeviceId }); // Composite key

            builder.Entity<RefreshToken>()
                .HasOne<User>()
                .WithMany() // User can have many RefreshTokens
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> RefreshToken

            // Configure PushToken
            builder.Entity<PushToken>()
                .HasKey(pt => new { pt.UserId, pt.DeviceId }); // Composite key

            builder.Entity<PushToken>()
                .HasOne<User>()
                .WithMany() // User can have many PushTokens
                .HasForeignKey(pt => pt.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> PushToken

            // Configure BaseActivities
            builder.Entity<ActivityBase>()
                .Property(a => a.Id) // Id is the primary key in BaseActivity
                .HasDefaultValueSql("uuid_generate_v4()");

            // Configure Activities
            builder.Entity<ActivityBase>()
                .HasDiscriminator<bool>("IsUserActivity")
                .HasValue<Activity>(false)
                .HasValue<UserActivity>(true);

            // Configure UserActivity
            builder.Entity<UserActivity>()
                .HasIndex(ua => new { ua.UserId, ua.Id }); // Composite index for UserId and and Id

            builder.Entity<UserActivity>()
                .HasIndex(ua => ua.UserId); // Index on UserId for efficient look

            builder.Entity<UserActivity>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserActivities) // User can have many UserActivities
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> UserActivity

            // Configure BaseFood
            builder.Entity<FoodBase>()
                .Property(f => f.Id) // Id is the primary key in BaseFood
                .HasDefaultValueSql("uuid_generate_v4()");

            // Configure Foods
            builder.Entity<FoodBase>()
                .HasDiscriminator<bool>("IsUserFood")
                .HasValue<Food>(false)
                .HasValue<UserFood>(true);

            // Configure UserFood
            builder.Entity<UserFood>()
                .HasIndex(uf => new { uf.UserId, uf.Id }); // Composite index for UserId and and Id

            builder.Entity<UserFood>()
                .HasIndex(uf => uf.UserId); // Index on UserId for efficient lookups

            builder.Entity<UserFood>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.UserFoods) // User can have many UserFoods
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> UserFood

            // Configure ActivityRecord
            builder.Entity<ActivityRecord>()
                .HasOne(ar => ar.User)
                .WithMany(u => u.ActivityRecords) // User can have many ActivityRecords
                .HasForeignKey(ar => ar.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> ActivityRecord

            // Configure ActivityRecord -> Activity relationship
            builder.Entity<ActivityRecord>()
                .HasOne(ar => ar.Activity)
                .WithMany() // ActivityBase can have many ActivityRecords
                .HasForeignKey(ar => ar.ActivityId)
                .OnDelete(DeleteBehavior.Restrict); // Don't cascade delete Activity -> ActivityRecord

            // Configure ConsumptionRecord
            builder.Entity<ConsumptionRecord>()
                .HasOne(cr => cr.User)
                .WithMany(u => u.ConsumptionRecords) // User can have many ConsumptionRecords
                .HasForeignKey(cr => cr.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> ConsumptionRecord

            builder.Entity<ConsumptionRecord>()
                .HasIndex(cr => cr.UserId); // Index on UserId for efficient lookups

            // Configure ConsumptionRecord -> Food relationship
            builder.Entity<ConsumptionRecord>()
                .HasOne(cr => cr.Food)
                .WithMany() // FoodBase can have many ConsumptionRecords
                .HasForeignKey(cr => cr.FoodId)
                .OnDelete(DeleteBehavior.Restrict); // Don't cascade delete Food -> ConsumptionRecord

            // Configure ConsumptionRecord -> UserMeal relationship
            builder.Entity<ConsumptionRecord>()
                .HasOne(cr => cr.UserMeal)
                .WithMany() // UserMeal can have many ConsumptionRecords
                .HasForeignKey(cr => cr.UserMealId)
                .OnDelete(DeleteBehavior.Restrict); // Don't cascade delete UserMeal -> ConsumptionRecord

            // Configure WaterIntakeRecord
            builder.Entity<WaterIntakeRecord>()
                .HasOne(wir => wir.User)
                .WithMany(u => u.WaterIntakeRecords) // User can have many WaterIntakeRecords
                .HasForeignKey(wir => wir.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> WaterIntakeRecord

            builder.Entity<WaterIntakeRecord>()
                .HasIndex(wir => wir.UserId); // Index on UserId for efficient look

            // Configure WeightRecord
            builder.Entity<WeightRecord>()
                .HasOne(wr => wr.User)
                .WithMany(u => u.WeightRecords) // User can have many WeightRecords
                .HasForeignKey(wr => wr.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> WeightRecord

            builder.Entity<WeightRecord>()
                .HasIndex(wr => wr.UserId); // Index on UserId for efficient lookups

            // Configure UserMeal
            builder.Entity<UserMeal>()
                .HasOne(um => um.User)
                .WithMany(u => u.UserMeals) // User can have many UserMeals
                .HasForeignKey(um => um.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> UserMeal

            builder.Entity<UserMeal>()
                .HasIndex(um => um.UserId); // Index on UserId for efficient lookups

            // Configure UserMealItem
            builder.Entity<UserMealItem>()
                .HasKey(umi => new { umi.UserMealId, umi.FoodId }); // Composite key

            builder.Entity<UserMealItem>()
                .HasOne(umi => umi.UserMeal)
                .WithMany(um => um.MealItems) // UserMeal can have many UserMealItems
                .HasForeignKey(umi => umi.UserMealId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for UserMeal -> UserMealItem

            builder.Entity<UserMealItem>()
                .HasOne(umi => umi.Food)
                .WithMany() // FoodBase can have many UserMealItems
                .HasForeignKey(umi => umi.FoodId)
                .OnDelete(DeleteBehavior.Restrict); // Don't cascade delete Food -> UserMealItem

            // Insert check to ensure either FoodId or UserMealId is set, but not both
            builder.Entity<ConsumptionRecord>()
                .ToTable(tb => tb.HasCheckConstraint("ck_consumption_record_food_id_user_meal_id",
                    "(food_id IS NOT NULL) != (user_meal_id IS NOT NULL)"));

            // Configure DailyBurnTarget
            builder.Entity<DailyBurnTarget>()
                .HasOne(dbt => dbt.User)
                .WithMany(u => u.DailyBurnTargets) // User can have many DailyBurnTargets
                .HasForeignKey(dbt => dbt.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> DailyBurnTarget

            builder.Entity<DailyBurnTarget>()
                .HasOne(dbt => dbt.Activity)
                .WithMany() // ActivityBase can have many DailyBurnTargets
                .HasForeignKey(dbt => dbt.ActivityId)
                .OnDelete(DeleteBehavior.Restrict); // Don't cascade delete Activity -> DailyBurnTarget

            builder.Entity<DailyBurnTarget>()
                .HasIndex(dbt => dbt.UserId); // Index on UserId for efficient lookups

            // Configure FriendRequest
            builder.Entity<FriendRequest>()
                .HasKey(fr => new { fr.RequesterId, fr.AddresseeId }); // Composite key

            builder.Entity<FriendRequest>()
                .HasOne(fr => fr.Requester)
                .WithMany(u => u.SentFriendRequests) // User can have many sent FriendRequests
                .HasForeignKey(fr => fr.RequesterId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> FriendRequest

            builder.Entity<FriendRequest>()
                .HasOne(fr => fr.Addressee)
                .WithMany(u => u.ReceivedFriendRequests) // User can have many received FriendRequests
                .HasForeignKey(fr => fr.AddresseeId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> FriendRequest

            builder.Entity<FriendRequest>()
                .HasIndex(fr => fr.AddresseeId); // Index on AddresseeId for efficient lookups

            builder.Entity<FriendRequest>()
                .HasIndex(fr => fr.RequesterId); // Index on RequesterId for efficient lookups

            // Configure Friendship
            builder.Entity<Friendship>()
                .HasKey(f => new { f.UserAId, f.UserBId }); // Composite key

            builder.Entity<User>()
                .HasMany(u => u.Friends)
                .WithMany()
                .UsingEntity<Friendship>(
                    j => j.HasOne(f => f.UserB)
                          .WithMany()
                          .HasForeignKey(f => f.UserBId)
                          .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne(f => f.UserA)
                          .WithMany()
                          .HasForeignKey(f => f.UserAId)
                          .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey(f => new { f.UserAId, f.UserBId }); // Composite key
                        j.HasIndex(f => f.UserBId); // Index on UserBId
                        j.HasIndex(f => f.UserAId); // Index on UserAId
                    });

            // Configure UserFollower
            builder.Entity<UserFollower>()
                .HasKey(uf => new { uf.FollowerId, uf.FolloweeId }); // Composite key

            builder.Entity<UserFollower>()
                .HasOne(uf => uf.Follower)
                .WithMany(u => u.Following) // User can follow many Users
                .HasForeignKey(uf => uf.FollowerId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> UserFollower

            builder.Entity<UserFollower>()
                .HasOne(uf => uf.Followee)
                .WithMany(u => u.Followers) // User can have many followers
                .HasForeignKey(uf => uf.FolloweeId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> UserFollower

            builder.Entity<UserFollower>()
                .HasIndex(uf => uf.FollowerId); // Index on FollowerId for efficient lookups

            builder.Entity<UserFollower>()
                .HasIndex(uf => uf.FolloweeId); // Index on FolloweeId for efficient lookups

            // Configure Post
            builder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts) // User can have many Posts
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> Post

            builder.Entity<Post>()
                .HasIndex(p => p.UserId); // Index on UserId for efficient lookups

            // Configure Comment
            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments) // User can have many Comments
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> Comment

            builder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments) // Post can have many Comments
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for Post -> Comment

            builder.Entity<Comment>()
                .HasIndex(c => c.PostId); // Index on PostId for efficient lookups

            // Configure PostLike
            builder.Entity<PostLike>()
                .HasKey(pl => new { pl.UserId, pl.PostId }); // Composite key

            builder.Entity<PostLike>()
                .HasOne(pl => pl.User)
                .WithMany() // User can have many PostLikes
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> PostLike

            builder.Entity<PostLike>()
                .HasOne(pl => pl.Post)
                .WithMany(p => p.Likes) // Post can have many PostLikes
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for Post -> PostLike

            builder.Entity<PostLike>()
                .HasIndex(pl => pl.PostId); // Index on PostId for efficient lookups

            // Configure PostImage
            builder.Entity<PostImage>()
                .HasKey(pi => new { pi.PostId, pi.Index }); // Composite key

            builder.Entity<PostImage>()
                .HasOne<Post>()
                .WithMany(p => p.Images) // Post can have many PostImages
                .HasForeignKey(pi => pi.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for Post -> PostImage

            // Configure Chat
            builder.Entity<Chat>()
                .HasMany(c => c.Members)
                .WithOne(cm => cm.Chat) // Chat has many ChatMembers
                .HasForeignKey(cm => cm.ChatId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for Chat -> ChatMember

            builder.Entity<Chat>()
                .HasMany(c => c.Messages)
                .WithOne(cm => cm.Chat) // Chat has many ChatMessages
                .HasForeignKey(cm => cm.ChatId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for Chat -> ChatMessage

            // Configure ChatMember
            builder.Entity<ChatMember>()
                .HasKey(cm => new { cm.ChatId, cm.UserId }); // Composite key

            builder.Entity<ChatMember>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.ChatMemberships) // User can have many ChatMemberships
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> ChatMember

            builder.Entity<ChatMember>()
                .HasIndex(cm => new { cm.ChatId, cm.UserId }); // Composite index for ChatId and UserId

            builder.Entity<ChatMember>()
                .HasIndex(cm => cm.UserId); // Index on UserId for efficient lookups

            builder.Entity<ChatMember>()
                .HasIndex(cm => cm.ChatId); // Index on ChatId for efficient lookups

            // TODO: Handle user deletion better
            // Configure ChatMessage
            builder.Entity<ChatMessage>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.ChatMessages) // User can have many ChatMessages
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User -> ChatMessage

            builder.Entity<ChatMessage>()
                .HasOne(cm => cm.Chat)
                .WithMany(c => c.Messages) // Chat can have many ChatMessages
                .HasForeignKey(cm => cm.ChatId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for Chat -> ChatMessage

            builder.Entity<ChatMessage>()
                .HasOne(cm => cm.SenderMembership)
                .WithMany(cm => cm.SentMessages) // ChatMember can have many sent ChatMessages
                .HasForeignKey(cm => new { cm.ChatId, cm.UserId }) // Foreign key to ChatMember using ChatId and UserId
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for ChatMember -> ChatMessage

            builder.Entity<ChatMessage>()
                .HasIndex(cm => cm.ChatId); // Index on ChatId for efficient lookups

            builder.Entity<ChatMessage>()
                .HasIndex(cm => cm.UserId); // Index on UserId for efficient lookups

            builder.Entity<ChatMessage>()
                .HasIndex(cm => cm.SentAt);
        }
    }
}
