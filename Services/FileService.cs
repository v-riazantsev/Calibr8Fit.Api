using Calibr8Fit.Api.Interfaces.Service;
using Microsoft.AspNetCore.StaticFiles;

namespace Calibr8Fit.Api.Services
{
    // Handles file operations including image validation, saving, and deletion
    public class FileService : IFileService
    {
        public bool IsImage(IFormFile file) => file.ContentType.StartsWith("image/");
        public bool IsExist(string path) => !string.IsNullOrEmpty(path) && File.Exists(path);
        public async Task<string> SaveImageAsync(IFormFile file, string savePath, string? fileName = null)
        {
            // Validate file is an image before saving to disk
            if (!IsImage(file))
                throw new InvalidOperationException("The uploaded file is not a valid image.");

            Directory.CreateDirectory(savePath);

            // Construct full path
            var fileExt = Path.GetExtension(file.FileName);
            fileName = fileName is null ? $"{Guid.NewGuid()}{fileExt}" : $"{fileName}{fileExt}";
            var fullPath = Path.Combine(savePath, fileName);

            // Save images
            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }
        public void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        public void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        public Stream GetFileStream(string path) =>
            new FileStream(path, FileMode.Open, FileAccess.Read);

        public string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            return provider.TryGetContentType(fileName, out var contentType)
                ? contentType
                : "application/octet-stream"; // fallback to binary
        }
    }
}