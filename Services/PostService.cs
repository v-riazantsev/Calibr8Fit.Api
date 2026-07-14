using Calibr8Fit.Api.DataTransferObjects.Post;
using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Services.Results;

namespace Calibr8Fit.Api.Services
{
    // Manages user posts, comments, likes, and image uploads
    public class PostService(
        IUserRepositoryBase<Post, Guid> postRepository,
        IUserRepositoryBase<Comment, Guid> commentRepository,
        IUserRepositoryBase<PostLike, (string, Guid)> postLikeRepository,
        IUserRepository userRepository,
        IPathService pathService,
        IFileService fileService
    ) : IPostService
    {
        private readonly IUserRepositoryBase<Post, Guid> _postRepository = postRepository;
        private readonly IUserRepositoryBase<Comment, Guid> _commentRepository = commentRepository;
        private readonly IUserRepositoryBase<PostLike, (string, Guid)> _postLikeRepository = postLikeRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPathService _pathService = pathService;
        private readonly IFileService _fileService = fileService;

        public async Task<Result<PostDto>> CreatePostAsync(CreatePostRequestDto createPostRequestDto, string userId)
        {
            // Resolve user ID to username for file path generation
            var username = (await _userRepository.GetAsync(userId))?.UserName;
            if (username is null) return Result<PostDto>.Failure("User not found");

            // Generate new post ID
            var postId = Guid.NewGuid();

            Console.WriteLine(createPostRequestDto.Images?.Count);

            // Save images and create PostImage entities
            var postImages = createPostRequestDto.Images is not null ?
                await Task.WhenAll(createPostRequestDto.Images.Select(
                    async (img, index) =>
                    {
                        // Save image to disk
                        await _fileService.SaveImageAsync(
                            img,
                            _pathService.GetPostImagesDirectoryPath(username, postId),
                            index.ToString()
                        );

                        // Return PostImage entity
                        return new PostImage
                        {
                            PostId = postId,
                            Index = index
                        };
                    }
            )) : [];

            // Create new post entity
            var post = new Post
            {
                Id = postId,
                UserId = userId,
                Content = createPostRequestDto.Content,
                Images = postImages
            };

            // Save post to repository
            var createdPost = await _postRepository.AddAsync(post);
            if (createdPost is null) return Result<PostDto>.Failure("Failed to create post");

            return Result<PostDto>.Success(await GetPostDtoAsync(createdPost, userId));
        }

        public async Task<Result<PostDto>> GetPostAsync(Guid postId, string? currentUserId = null)
        {
            // Retrieve the post
            var post = await _postRepository.GetAsync(postId);
            if (post is null) return Result<PostDto>.Failure("Post not found");

            return Result<PostDto>.Success(await GetPostDtoAsync(post, currentUserId));
        }

        public async Task<Result<IEnumerable<PostDto>>> GetPostsByUserIdAsync(string userId, string? currentUserId = null)
        {
            // Retrieve posts by user
            var posts = await GetAllPosts(userId);
            if (!posts.Any()) return Result<IEnumerable<PostDto>>.Success([]);

            return Result<IEnumerable<PostDto>>.Success(
                await Task.WhenAll(posts.Select(async post => await GetPostDtoAsync(post, currentUserId)))
            );
        }

        public async Task<Result<IEnumerable<PostDto>>> GetPostsByUserNameAsync(string username, string? currentUserId = null)
        {
            var userId = await _userRepository.GetIdByUsernameAsync(username);
            if (userId is null) return Result<IEnumerable<PostDto>>.Failure("User not found");

            return await GetPostsByUserIdAsync(userId, currentUserId);
        }

        public async Task<Result<IEnumerable<PostDto>>> GetLatestPostsByUserIdAsync(string userId, int page, int size, string? currentUserId = null)
        {
            // Retrieve posts by user with pagination
            var posts = await GetLatestPosts(userId, page, size);
            if (!posts.Any()) return Result<IEnumerable<PostDto>>.Success([]);

            // TODO: improve performance
            return Result<IEnumerable<PostDto>>.Success(
                posts.Select(async post => await GetPostDtoAsync(post, currentUserId)).Select(t => t.Result)
            );
        }

        public async Task<Result<IEnumerable<PostDto>>> GetLatestPostsByUserNameAsync(string username, int page, int size, string? currentUserId = null)
        {
            var userId = await _userRepository.GetIdByUsernameAsync(username);
            if (userId is null) return Result<IEnumerable<PostDto>>.Failure("User not found");

            return await GetLatestPostsByUserIdAsync(userId, page, size, currentUserId);
        }

        public async Task<Result<IEnumerable<PostDto>>> GetFeedPostsAsync(User user, int page, int size)
        {
            if (user.Following!.Count == 0) return Result<IEnumerable<PostDto>>.Success([]);

            // Get IDs of followed users
            var followedUserIdsSet = user.Following.Select(f => f.FolloweeId).ToHashSet();

            // Retrieve posts from followed users with pagination
            var posts = await _postRepository.QueryAsync(q =>
                q.Where(p => followedUserIdsSet.Contains(p.UserId))
                .OrderByDescending(p => p.CreatedAt)
                .Skip(page * size)
                .Take(size)
            );

            // Return post dtos
            return Result<IEnumerable<PostDto>>.Success(
                posts.Select(async post => await GetPostDtoAsync(post, user.Id)).Select(t => t.Result)
            );
        }
        public async Task<Result> DeletePostAsync(Guid postId, string userId)
        {
            // Retrieve the post to ensure it exists and belongs to the user
            var post = await _postRepository.GetByUserIdAndKeyAsync(userId, postId);
            if (post is null) return Result.Failure("Post not found");

            // Delete associated images from disk
            _fileService.DeleteDirectory(_pathService.GetPostDirectoryPath(post.User!.UserName!, postId));

            // Delete the post
            var deleted = await _postRepository.DeleteAsync(postId);
            if (deleted is null) return Result.Failure("Failed to delete post");


            return Result.Success();
        }
        public async Task<Result> LikePostAsync(Guid postId, string userId)
        {
            // Check if the post exists
            var post = await _postRepository.GetAsync(postId);
            if (post is null) return Result.Failure("Post not found");

            // Check if the user has already liked the post
            var existingLike = await _postLikeRepository.GetAsync(userId, postId);
            if (existingLike is not null) return Result.Failure("Post already liked");

            // Create new like
            var postLike = new PostLike
            {
                UserId = userId,
                PostId = postId
            };

            var createdLike = await _postLikeRepository.AddAsync(postLike);
            if (createdLike is null) return Result.Failure("Failed to like post");

            return Result.Success();
        }

        public async Task<Result> UnlikePostAsync(Guid postId, string userId)
        {
            // Check if the post exists
            var post = await _postRepository.GetAsync(postId);
            if (post is null) return Result.Failure("Post not found");

            var deletedLike = await _postLikeRepository.DeleteAsync(userId, postId);
            if (deletedLike is null) return Result.Failure("Post not liked");

            return Result.Success();
        }

        public async Task<Result<IEnumerable<CommentDto>>> GetCommentsAsync(Guid postId, int page, int size)
        {
            // Check if the post exists
            var post = await _postRepository.GetAsync(postId);
            if (post is null) return Result<IEnumerable<CommentDto>>.Failure("Post not found");

            // Retrieve comments with pagination
            var comments = await _commentRepository.QueryAsync(q =>
                q.Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt)
                .Skip(page * size)
                .Take(size)
            );

            return Result<IEnumerable<CommentDto>>.Success(
                comments.Select(c => c.ToCommentDto(_pathService))
            );
        }

        public async Task<Result<CommentDto>> AddCommentAsync(Guid postId, string content, string userId)
        {
            // Check if the post exists
            var post = await _postRepository.GetAsync(postId);
            if (post is null) return Result<CommentDto>.Failure("Post not found");

            // Create new comment
            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                Content = content
            };

            var createdComment = await _commentRepository.AddAsync(comment);
            if (createdComment is null) return Result<CommentDto>.Failure("Failed to add comment");

            return Result<CommentDto>.Success(createdComment.ToCommentDto(_pathService));
        }

        public async Task<Result> DeleteCommentAsync(Guid commentId, string userId)
        {
            // Retrieve the comment to ensure it exists and belongs to the user
            var comment = await _commentRepository.GetByUserIdAndKeyAsync(userId, commentId);
            if (comment is null) return Result.Failure("Comment not found");

            // Delete the comment
            var deleted = await _commentRepository.DeleteAsync(commentId);
            if (deleted is null) return Result.Failure("Failed to delete comment");

            return Result.Success();
        }

        private async Task<IEnumerable<Post>> GetLatestPosts(string userId, int page, int size) =>
            await _postRepository.QueryAsync(q =>
                q.Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip(page * size)
                .Take(size)
            );
        private async Task<IEnumerable<Post>> GetAllPosts(string userId) =>
            await _postRepository.GetAllByUserIdAsync(userId);
        // TODO: improve performance
        private async Task<PostDto> GetPostDtoAsync(Post post, string? currentUserId = null)
        {
            var likeCount = await _postLikeRepository.CountAsync(pl => pl.PostId == post.Id);
            var commentCount = await _commentRepository.CountAsync(c => c.PostId == post.Id);
            var isLikedByCurrentUser = currentUserId is not null &&
                await _postLikeRepository.KeyExistsAsync(currentUserId, post.Id);

            return post.ToPostDto(
                likeCount,
                commentCount,
                isLikedByCurrentUser,
                _pathService
            );
        }
    }
}
