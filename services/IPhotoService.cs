using System;
using API.utils;

namespace API.services;

public interface IPhotoService
{
    Task<PhotoUploadResult> UploadImageAsync(IFormFile file);
    Task<bool> DeleteImageAsync(string publicId);
}
