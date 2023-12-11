using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Images.Models
{
    public record UploadImageModel(IFormFile File, int ProductId);
}
