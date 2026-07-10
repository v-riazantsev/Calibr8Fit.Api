using Calibr8Fit.Api.Data;
using Calibr8Fit.Api.Enums;
using Calibr8Fit.Api.Extensions;
using Calibr8Fit.Api.Hubs;
using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Options;
using Calibr8Fit.Api.Repository;
using Calibr8Fit.Api.Repository.Base;
using Calibr8Fit.Api.Services;
using Calibr8Fit.Api.Validators;
using DotNetEnv;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

// Load environment variables from .env file
Env.Load();

// Configure DbContext with PostgreSQL connection
builder.Services.AddDbContext<ApplicationDbContext>(options => options
    .UseSnakeCaseNamingConvention()
    .UseLazyLoadingProxies()
    .UseNpgsql(Environment.GetEnvironmentVariable("DefaultConnection")));

// Custom validator
builder.Services.AddScoped<IUserValidator<User>>(provider =>
    new UserNameLengthValidator<User>(5, 32));

// Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;

    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";
})
.AddDefaultTokenProviders()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));

// Firebase
var credential = GoogleCredential.FromFile(Environment.GetEnvironmentVariable("FirebaseCredentialPath"));
FirebaseApp.Create(new AppOptions { Credential = credential });

builder.Services.AddHttpContextAccessor();

// Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPushService, PushService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<ITPHValidationService<Guid, Activity, UserActivity>, TPHValidationService<Guid, Activity, UserActivity>>();
builder.Services.AddScoped<ITPHValidationService<Guid, Food, UserFood>, TPHValidationService<Guid, Food, UserFood>>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<IFollowingService, FollowingService>();
builder.Services.AddScoped<IPathService, PathService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessengerService, MessengerService>();
builder.Services.AddScoped<IChatNotifier, ChatNotifier>();

// Singleton Services
builder.Services.AddSingleton<IOnlineTracker, OnlineTracker>();
builder.Services.AddSingleton<IChatActivityTracker, ChatActivityTracker>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserRepositoryBase<RefreshToken, string[]>, UserRepositoryBase<RefreshToken, string[]>>();
builder.Services.AddScoped<IUserRepositoryBase<PushToken, string[]>, UserRepositoryBase<PushToken, string[]>>();
builder.Services.AddScoped<IRepositoryBase<UserProfile, string>, RepositoryBase<UserProfile, string>>();
builder.Services.AddScoped<IUserRepositoryBase<ProfilePicture, string[]>, UserRepositoryBase<ProfilePicture, string[]>>();
builder.Services.AddScoped<IDataVersionRepository, DataVersionRepository>();
builder.Services.AddScoped<IRepositoryBase<Activity, Guid>, RepositoryBase<Activity, Guid>>();
builder.Services.AddScoped<IRepositoryBase<Food, Guid>, RepositoryBase<Food, Guid>>();
builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();
builder.Services.AddScoped<IRepositoryBase<FriendRequest, string[]>, RepositoryBase<FriendRequest, string[]>>();
builder.Services.AddScoped<IUserRepositoryBase<UserFollower, (string, string)>, UserRepositoryBase<UserFollower, (string, string)>>();
builder.Services.AddScoped<IUserRepositoryBase<Post, Guid>, UserRepositoryBase<Post, Guid>>();
builder.Services.AddScoped<IUserRepositoryBase<Comment, Guid>, UserRepositoryBase<Comment, Guid>>();
builder.Services.AddScoped<IUserRepositoryBase<PostLike, (string, Guid)>, UserRepositoryBase<PostLike, (string, Guid)>>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IUserRepositoryBase<ChatMember, (Guid, string)>, UserRepositoryBase<ChatMember, (Guid, string)>>();
builder.Services.AddScoped<IChatMessagesRepository, ChatMessagesRepository>();

// Data Version Repositories
builder.Services.AddDataVersionRepo<Food, Guid>(DataResource.Foods);
builder.Services.AddDataVersionRepo<Activity, Guid>(DataResource.Activities);

// Sync Repositories
builder.Services.AddUserSyncRepo<UserActivity, Guid>();
builder.Services.AddUserSyncRepo<ActivityRecord, Guid>();
builder.Services.AddUserSyncRepo<ConsumptionRecord, Guid>();
builder.Services.AddUserSyncRepo<UserFood, Guid>();
builder.Services.AddUserSyncRepo<WaterIntakeRecord, Guid>();
builder.Services.AddUserSyncRepo<WeightRecord, Guid>();
builder.Services.AddUserSyncRepo<UserMeal, Guid>();
builder.Services.AddUserSyncRepo<DailyBurnTarget, Guid>();

builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<MessengerHub>("/hubs/chat");

app.Run();
