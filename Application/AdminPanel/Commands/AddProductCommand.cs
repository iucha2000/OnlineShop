using Application.Common.Persistence;
using Application.Services;
using Domain;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminPanel.Commands
{
    public record AddProductCommand(string Name, decimal Price, int ProductId, ProductCategory Category, IFormFile Image, int Count): IRequest<Result>;

    internal class AddProductCommandHandler : IRequestHandler<AddProductCommand, Result>
    {
        private readonly IGenericRepository<Domain.Entities.Product> _productRepository;
        private readonly IGenericRepository<Domain.Entities.Image> _imageRepository;
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public AddProductCommandHandler(IGenericRepository<Domain.Entities.Product> productRepository, IUnitOfWork unitOfWork, IGenericRepository<Image> imageRepository, IImageService imageService)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _imageRepository = imageRepository;
            _imageService = imageService;
        }

        public async Task<Result> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            for (int i = 0; i < request.Count; i++)
            {
                var product = new Domain.Entities.Product
                {
                    Name = request.Name,
                    Price = request.Price,
                    ProductId = request.ProductId,
                    Category = request.Category,
                    OrderId = null,
                    IsSold = false
                };

                await _productRepository.AddAsync(product);
            }

            var imageUpload = _imageService.UploadImageAsync(request.Image);
            var image = new Image
            {
                ProductId = request.ProductId,
                PublicId = imageUpload.Result.PublicId,
                Url = imageUpload.Result.Url,
            };
            await _imageRepository.AddAsync(image);

            await _unitOfWork.CommitAsync();

            return Result.Succeed();
        }
    }
}
