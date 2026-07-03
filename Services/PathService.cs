using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Options;
using Microsoft.Extensions.Options;

namespace Calibr8Fit.Api.Services
{
    public class PathService(
        IHttpContextAccessor httpContextAccessor,
        IOptions<StorageOptions> options
        ) : IPathService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly StorageOptions _options = options.Value;

        private static string EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
        public string GetUserUploadsDirectoryPath(string username) =>
            EnsureDirectoryExists(Path.Combine(
                _options.RootPath,
                _options.UploadsPath,
                username));
        public string GetProfilePictureDirectoryPath(string username) =>
            EnsureDirectoryExists(Path.Combine(
                    GetUserUploadsDirectoryPath(username),
                    _options.ProfilePicturesSubfolder
            ));
        public string? GetProfilePicturePath(string username, string fileName)
        {
            var path = Path.Combine(
                GetProfilePictureDirectoryPath(username),
                fileName
            );

            // Return path if it exists
            return File.Exists(path) ? path : null;
        }
        public string? GetProfilePictureUrl(string username, string fileName)
        {
            var path = GetProfilePicturePath(username, fileName);
            // If path exists return public url
            return path == null ? null : BuildPublicUrl(path);
        }
        public string GetPostDirectoryPath(string username, Guid postId) =>
            EnsureDirectoryExists(Path.Combine(
                GetUserUploadsDirectoryPath(username)!,
                _options.PostSubfolder,
                postId.ToString()
            ));
        public string GetPostImagesDirectoryPath(string username, Guid postId) =>
            EnsureDirectoryExists(Path.Combine(
                GetPostDirectoryPath(username, postId)!,
                _options.PostImagesSubfolder
            ));
        public string? GetPostImageUrl(string username, Guid postId, int index)
        {
            var directoryPath = GetPostImagesDirectoryPath(username, postId);
            var imagePath = Directory.GetFiles(directoryPath, $"{index}.*");

            if (imagePath.Length == 0 || !File.Exists(imagePath[0])) return null;

            return BuildPublicUrl(imagePath[0]);
        }
        public string RemoveRoot(string path) =>
            path.StartsWith(_options.RootPath) ?
                path.Substring(_options.RootPath.Length + 1) :
                path;
        public string AddRoot(string path) =>
            path.StartsWith(_options.RootPath) ?
                path :
                Path.Combine(_options.RootPath, path);
        public string BuildPublicUrl(string relativePath)
        {
            // Get current HTTP request
            var request = _httpContextAccessor.HttpContext?.Request;

            if (request is null)
                throw new InvalidOperationException("No active HTTP request context.");

            // Normalize path and remove leading "wwwroot/" if present
            var cleanedPath = relativePath.Replace("\\", "/");
            if (cleanedPath.StartsWith(_options.RootPath))
                cleanedPath = cleanedPath.Substring($"{_options.RootPath}/".Length);
            // Return url
            return $"{request.Scheme}://{request.Host}/{cleanedPath}";
        }
        public string? PublicUrlToRelativePath(string url)
        {
            // Generate path
            var path = Path.Combine(_options.RootPath, Path.Combine(new Uri(url).LocalPath.Split('/')));
            // Return path if it exists
            return File.Exists(path) ? path : null;
        }
        public string GetChatDirectoryPath(Guid chatId) =>
            EnsureDirectoryExists(Path.Combine(
                _options.RootPath,
                _options.ChatPath,
                chatId.ToString()
            ));
        public string? GetChatAvatarPath(Guid chatId) =>
            Path.Combine(
                GetChatDirectoryPath(chatId),
                _options.ChatAvatarName
            );

        public string? GetChatAvatarUrl(Guid chatId)
        {
            var path = GetChatAvatarPath(chatId);
            return path == null ? null : BuildPublicUrl(path);
        }
    }
}