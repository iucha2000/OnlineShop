using Application.Images.Models;
using Application.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ImageUploader : IImageService
    {
        private readonly Cloudinary cloudinary;

        public ImageUploader(IOptions<CloudinarySettings> settings)
        {
            cloudinary = new Cloudinary(new Account(
                settings.Value.CloudName,
                settings.Value.ApiKey,
                settings.Value.ApiSecret));
        }

        public async Task<UploadImageResult> UploadImageAsync(IFormFile file)
        {
            var result = new ImageUploadResult();

            if(file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, stream)
                };

                result = await cloudinary.UploadAsync(uploadParams);
            }
            
            return new UploadImageResult() { PublicId = result.PublicId, Url = result.Url.ToString() };
        }
    }
}
