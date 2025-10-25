using System;
using API.utils;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace API.services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }
    public async Task<PhotoUploadResult?> UploadImageAsync(IFormFile file)
{
    if (file == null || file.Length == 0)
        return null;

    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
    var ext = Path.GetExtension(file.FileName).ToLower();

    if (!allowedExtensions.Contains(ext))
        throw new Exception("Only .jpg, .jpeg, .png images are allowed");

    if (file.Length > 2 * 1024 * 1024)
        throw new Exception("Max allowed size is 2 MB");

    var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileNameWithoutExtension(file.FileName)}";

    try
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, file.OpenReadStream()),
            PublicId = $"chat-app-images/{uniqueName}",
            Folder = "chat-app-images"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
            throw new Exception(uploadResult.Error.Message);

        return new PhotoUploadResult
        {
            Url = uploadResult.SecureUrl.ToString(),
            PublicId = uploadResult.PublicId
        };
    }
    catch (Exception ex)
    {
        throw new Exception($"Image upload failed: {ex.Message}");
    }
}

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        return result.Result == "ok";
    }
}
