using System.Text;
using Calibr8Fit.Api.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Calibr8Fit.Api.Tests.Unit.Services
{
    public class FileServiceTests
    {
        private readonly FileService _service;

        public FileServiceTests()
        {
            _service = new FileService();
        }
        [Theory]
        [InlineData("image/jpeg", true)]
        [InlineData("image/png", true)]
        [InlineData("application/pdf", false)]
        public void FileService_IsImage_ReturnsExpected(string contentType, bool expected)
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.ContentType).Returns(contentType);

            // Act
            var result = _service.IsImage(mockFile.Object);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task FileService_SaveImageAsync_ThrowsForNonImage()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.ContentType).Returns("application/pdf");

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.SaveImageAsync(mockFile.Object, Path.GetTempPath()));

            // Assert
            Assert.Equal("The uploaded file is not a valid image.", exception.Message);
        }

        [Theory]
        [InlineData("test", "image/png")]
        [InlineData("test", "image/jpeg")]
        [InlineData(null, "image/jpeg")]
        public async Task FileService_SaveImageAsync_SavesFileAndReturnsName(
            string? fileName,
            string contentType)
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            var content = "fake image content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("filename.png");
            mockFile.Setup(f => f.ContentType).Returns(contentType);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                    .Returns<Stream, CancellationToken>((s, _) => stream.CopyToAsync(s));

            try
            {
                // Act
                var result = await _service.SaveImageAsync(mockFile.Object, tempDir, fileName);
                var filePath = Path.Combine(tempDir, result);

                // Assert
                Assert.True(File.Exists(filePath));
                if (fileName is not null)
                    Assert.StartsWith(fileName, Path.GetFileNameWithoutExtension(result));
            }
            finally
            {
                // Cleanup
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void FileService_GetContentType_ReturnsExpectedTypes()
        {
            // Act + Assert
            Assert.Equal("image/jpeg", _service.GetContentType("photo.jpg")); // known type
            Assert.Equal("application/octet-stream", _service.GetContentType("unknownfile.xyz")); // fallback
        }
        [Fact]
        public void FileService_DeleteFile_RemovesExistingFile()
        {
            // Arrange: create a temp file
            var tempFile = Path.GetTempFileName();

            // Act
            _service.DeleteFile(tempFile);

            // Assert
            Assert.False(File.Exists(tempFile));
        }
        [Fact]
        public void FileService_DeleteDirectory_RemovesExistingDirectory()
        {
            // Arrange: create a temp directory with a file in it
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(Path.Combine(tempDir, "dummy.txt"), "data");

            // Act
            _service.DeleteDirectory(tempDir);

            // Assert
            Assert.False(Directory.Exists(tempDir));
        }
        [Fact]
        public void FileService_GetFileStream_ReturnsReadableStream()
        {
            // Arrange: create a temp file with some content
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "hello test");

            try
            {
                // Act
                using var stream = _service.GetFileStream(tempFile);
                using var reader = new StreamReader(stream);
                var content = reader.ReadToEnd();

                // Assert
                Assert.Equal("hello test", content);
            }
            finally
            {
                // Cleanup
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
        [Fact]
        public void FileService_IsExist_ReturnTrue()
        {
            // Arrange: create a temp directory with a file in it
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var filePath = Path.Combine(tempDir, "dummy.txt");
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(filePath, "data");

            // Act
            var result = _service.IsExist(filePath);

            // Assert
            Assert.True(result);

            // Cleanup
            if (File.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
        [Theory]
        [InlineData("some/file.txt")]
        [InlineData("")]
        public void FileService_IsExist_ReturnFalse(string filePath)
        {
            // Act
            var result = _service.IsExist(filePath);

            // Assert
            Assert.False(result);
        }
    }
}