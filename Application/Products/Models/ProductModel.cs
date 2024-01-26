using Application.Comments.Models;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Product.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public ProductCategory Category { get; set; }
        public string? Url { get; set; }
        public List<CommentModel> Comments { get; set; }
    }
}
