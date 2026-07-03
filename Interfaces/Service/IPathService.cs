namespace Calibr8Fit.Api.Interfaces.Service
{
    public interface IPathService
    {
        string GetUserUploadsDirectoryPath(string username);
        string GetProfilePictureDirectoryPath(string username);
        string? GetProfilePicturePath(string username, string fileName);
        string? GetProfilePictureUrl(string username, string fileName);
        string GetPostDirectoryPath(string username, Guid postId);
        string GetPostImagesDirectoryPath(string username, Guid postId);
        string? GetPostImageUrl(string username, Guid postId, int index);
        string GetChatDirectoryPath(Guid chatId);
        string? GetChatAvatarPath(Guid chatId);
        string? GetChatAvatarUrl(Guid chatId);
        string RemoveRoot(string path);
        string AddRoot(string path);
        string BuildPublicUrl(string relativePath);
        string? PublicUrlToRelativePath(string url);
    }
}