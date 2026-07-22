using Calibr8Fit.Api.Options;
using Calibr8Fit.Api.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Calibr8Fit.Api.Tests.Unit.Services
{
    public class PathServiceTests
    {
        private readonly PathService _service;
        private readonly StorageOptions _options;
        private readonly HttpRequest _request;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string RootPath = "testroot";
        public PathServiceTests()
        {
            var mockOptions = Microsoft.Extensions.Options.Options.Create(new StorageOptions
            {
                RootPath = RootPath,
                ChatPath = "chat",
                ChatAvatarName = "avatar.png",
                UploadsPath = "uploads",
                DownloadsPath = "downloads",
                ProfilePicturesSubfolder = "profile-pictures",
                PostSubfolder = "posts",
                PostImagesSubfolder = "images"
            });

            _options = mockOptions.Value;

            // Mock HttpRequest
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Scheme).Returns("https");
            mockRequest.Setup(r => r.Host).Returns(new HostString("localhost"));

            _request = mockRequest.Object;

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(new DefaultHttpContext
            {
                Request = { Scheme = "https", Host = new HostString("localhost") }
            });
            _httpContextAccessor = mockHttpContextAccessor.Object;

            _service = new PathService(_httpContextAccessor, mockOptions);
        }

        [Fact]
        public void PathService_GetUserUploadsDirectoryPath_ReturnString()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();
            var userUploadPath = Path.Combine(
                _options.RootPath,
                _options.UploadsPath,
                username);

            // Act
            var result = _service.GetUserUploadsDirectoryPath(username);

            // Assert
            Assert.Equal(userUploadPath, result);
            Assert.True(Directory.Exists(userUploadPath));

            // Cleanup
            if (Directory.Exists(userUploadPath))
                Directory.Delete(userUploadPath, true);
        }

        [Fact]
        public void PathService_GetProfilePictureDirectoryPath_ReturnString()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();
            var userProfilePictureDirectoryPath = Path.Combine(
                _options.RootPath,
                _options.UploadsPath,
                username,
                _options.ProfilePicturesSubfolder);


            // Act
            var result = _service.GetProfilePictureDirectoryPath(username);

            // Assert
            Assert.Equal(userProfilePictureDirectoryPath, result);
            Assert.True(Directory.Exists(userProfilePictureDirectoryPath));

            // Cleanup
            if (Directory.Exists(userProfilePictureDirectoryPath))
                Directory.Delete(userProfilePictureDirectoryPath, true);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PathService_GetProfilePicturePath_ReturnStringOrNull(bool createFile)
        {
            // Arrange
            var profilePictureName = "test-profile-picture.png";
            var username = Guid.NewGuid().ToString();

            var userProfilePictureDirectoryPath = Path.Combine(
                _options.RootPath,
                _options.UploadsPath,
                username,
                _options.ProfilePicturesSubfolder);
            var fullFilePath = Path.Combine(userProfilePictureDirectoryPath, profilePictureName);

            // Create the directory and file
            Directory.CreateDirectory(userProfilePictureDirectoryPath);
            if (createFile)
                File.WriteAllText(fullFilePath, "fake image content");

            // Act
            var result = _service.GetProfilePicturePath(username, profilePictureName);

            // Assert
            if (createFile)
                Assert.Equal(fullFilePath, result);
            else
                Assert.Null(result);

            // Cleanup
            if (Directory.Exists(userProfilePictureDirectoryPath))
                Directory.Delete(userProfilePictureDirectoryPath, true);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PathService_GetProfilePictureUrl_ReturnStringOrNull(bool createFile)
        {
            // Arrange
            var profilePictureName = "test-profile-picture.png";
            var username = Guid.NewGuid().ToString();

            var userProfilePictureDirectoryPath = Path.Combine(
                _options.RootPath,
                _options.UploadsPath,
                username,
                _options.ProfilePicturesSubfolder);
            var fullFilePath = Path.Combine(userProfilePictureDirectoryPath, profilePictureName);

            // Create the directory and file
            Directory.CreateDirectory(userProfilePictureDirectoryPath);
            if (createFile)
                File.WriteAllText(fullFilePath, "fake image content");

            // Act
            var result = _service.GetProfilePictureUrl(username, profilePictureName);

            // Assert
            if (createFile)
            {
                Assert.NotNull(result);
                Assert.StartsWith("https://", result);
                Assert.Contains(profilePictureName, result);
            }
            else
                Assert.Null(result);

            // Cleanup
            if (Directory.Exists(userProfilePictureDirectoryPath))
                Directory.Delete(userProfilePictureDirectoryPath, true);
        }

        [Fact]
        public void PathService_GetPostImagesDirectoryPath_ReturnString()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();
            var postId = Guid.NewGuid();

            var userPostsImagesDirectoryPath = Path.Combine(
                _options.RootPath,
                _options.UploadsPath,
                username,
                _options.PostSubfolder,
                postId.ToString(),
                _options.PostImagesSubfolder);

            // Act
            var result = _service.GetPostImagesDirectoryPath(username, postId);

            // Assert
            Assert.Equal(userPostsImagesDirectoryPath, result);
            Assert.True(Directory.Exists(userPostsImagesDirectoryPath));

            // Cleanup
            if (Directory.Exists(userPostsImagesDirectoryPath))
                Directory.Delete(userPostsImagesDirectoryPath, true);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PathService_GetPostImagePath_ReturnStringOrNull(bool createFile)
        {
            // Arrange
            var postImageIndex = 1;
            var username = Guid.NewGuid().ToString();
            var postId = Guid.NewGuid();

            var userPostsImagesDirectoryPath = Path.Combine(
                _options.RootPath,
                _options.UploadsPath,
                username,
                _options.PostSubfolder,
                postId.ToString(),
                _options.PostImagesSubfolder);
            var fullFilePath = Path.Combine(userPostsImagesDirectoryPath, postImageIndex.ToString());

            // Create the directory and file
            Directory.CreateDirectory(userPostsImagesDirectoryPath);
            if (createFile)
                File.WriteAllText(fullFilePath, "fake image content");

            // Act
            var result = _service.GetPostImageUrl(username, postId, postImageIndex);

            // Assert
            if (createFile)
            {
                Assert.NotNull(result);
                Assert.StartsWith("https://", result);
                Assert.Contains(postImageIndex.ToString(), result);
            }
            else
                Assert.Null(result);

            // Cleanup
            if (Directory.Exists(userPostsImagesDirectoryPath))
                Directory.Delete(userPostsImagesDirectoryPath, true);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PathService_GetPostImageUrl_ReturnStringOrNull(bool createFile)
        {
            // Arrange
            var postImageIndex = 1;
            var username = Guid.NewGuid().ToString();
            var postId = Guid.NewGuid();

            var userPostsImagesDirectoryPath = Path.Combine(
                _options.RootPath,
                _options.UploadsPath,
                username,
                _options.PostSubfolder,
                postId.ToString(),
                _options.PostImagesSubfolder);
            var fullFilePath = Path.Combine(userPostsImagesDirectoryPath, postImageIndex.ToString());

            // Create the directory and file
            Directory.CreateDirectory(userPostsImagesDirectoryPath);
            if (createFile)
                File.WriteAllText(fullFilePath, "fake image content");

            // Act
            var result = _service.GetPostImageUrl(username, postId, postImageIndex);

            // Assert
            if (createFile)
            {
                Assert.NotNull(result);
                Assert.StartsWith("https://", result);
                Assert.Contains(postImageIndex.ToString(), result);
            }
            else
                Assert.Null(result);

            // Cleanup
            if (Directory.Exists(userPostsImagesDirectoryPath))
                Directory.Delete(userPostsImagesDirectoryPath, true);
        }

        [Theory]
        [InlineData($"{RootPath}/some/path", "some/path")]
        [InlineData($"{RootPath}\\some\\path", "some\\path")]
        [InlineData("some/path", "some/path")]
        [InlineData("\\some\\path", "\\some\\path")]
        public void PathService_RemoveRoot_ReturnString(string path, string expected)
        {
            // Act
            var result = _service.RemoveRoot(path);

            // Assert
            Assert.Equal(result, expected);
        }

        [Fact]
        public void PathService_AddRoot_WithoutRoot_ReturnString()
        {
            //Arrange
            var path = Path.Combine("some", "path");

            // Act
            var result = _service.AddRoot(path);

            // Assert
            Assert.Equal(result, Path.Combine(RootPath, path));
        }

        [Fact]
        public void PathService_AddRoot_WithRoot_ReturnString()
        {
            //Arrange
            var path = Path.Combine(RootPath, "some", "path");

            // Act
            var result = _service.AddRoot(path);

            // Assert
            Assert.Equal(result, path);
        }

        [Fact]
        public void PathService_BuildPublicUrl_ReturnString()
        {
            // Arrange
            var path = Path.Combine("some", "path");

            // Act
            var result = _service.BuildPublicUrl(path);

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith("https://", result);
            Assert.Contains(path, result);
        }

        [Fact]
        public void PathService_BuildPublicUrl_ThrowsArgumentNullException_WhenRequestIsNull()
        {
            // Arrange
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(a => a.HttpContext).Returns((HttpContext?)null);
            var service = new PathService(mockHttpContextAccessor.Object, Microsoft.Extensions.Options.Options.Create(_options));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => service.BuildPublicUrl("some/path"));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PathService_PublicUrlToRelativePath_ReturnStringOrNull(bool createFile)
        {
            // Arrange
            var fileName = "test-image.png";

            var testDirectoryPath = Path.Combine(
                _options.RootPath,
                "some",
                "path");
            var fullFilePath = Path.Combine(testDirectoryPath, fileName);

            // Create the directory and file
            Directory.CreateDirectory(testDirectoryPath);
            if (createFile)
                File.WriteAllText(fullFilePath, "fake image content");

            // Construct a matching public URL
            var relativePath = Path.GetRelativePath(_options.RootPath, fullFilePath);
            var publicUrl = $"https://localhost/{relativePath.Replace("\\", "/")}";

            // Act
            var result = _service.PublicUrlToRelativePath(publicUrl);

            // Assert
            if (createFile)
            {
                Assert.Equal(fullFilePath, result);
            }
            else
                Assert.Null(result);

            // Cleanup
            if (Directory.Exists(testDirectoryPath))
                Directory.Delete(testDirectoryPath, true);
        }

        [Fact]
        public void PathService_GetChatDirectoryPath_ReturnString()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var expectedPath = Path.Combine(
                _options.RootPath,
                _options.ChatPath,
                chatId.ToString());

            // Act
            var result = _service.GetChatDirectoryPath(chatId);

            // Assert
            Assert.Equal(expectedPath, result);
            Assert.True(Directory.Exists(expectedPath));

            // Cleanup
            if (Directory.Exists(expectedPath))
                Directory.Delete(expectedPath, true);
        }

        [Fact]
        public void PathService_GetChatAvatarPath_ReturnString()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var expectedPath = Path.Combine(
                _options.RootPath,
                _options.ChatPath,
                chatId.ToString(),
                _options.ChatAvatarName);

            // Create the directory and file
            Directory.CreateDirectory(Path.GetDirectoryName(expectedPath)!);
            File.WriteAllText(expectedPath, "fake avatar content");

            // Act
            var result = _service.GetChatAvatarPath(chatId);

            // Assert
            Assert.Equal(expectedPath, result);
        }

        [Fact]
        public void PathService_GetChatAvatarPath_ReturnNull_WhenFileDoesNotExist()
        {
            // Arrange && Act
            var result = _service.GetChatAvatarPath(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void PathService_GetChatAvatarUrl_ReturnString()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var expectedPath = Path.Combine(
                _options.RootPath,
                _options.ChatPath,
                chatId.ToString(),
                _options.ChatAvatarName);

            // Create the directory and file
            Directory.CreateDirectory(Path.GetDirectoryName(expectedPath)!);
            File.WriteAllText(expectedPath, "fake avatar content");

            // Act
            var result = _service.GetChatAvatarUrl(chatId);

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith("https://", result);
            Assert.Contains(_options.ChatAvatarName, result);
        }

        [Fact]
        public void PathService_GetChatAvatarUrl_ReturnNull_WhenFileDoesNotExist()
        {
            // Arrange && Act
            var result = _service.GetChatAvatarUrl(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }
    }
}