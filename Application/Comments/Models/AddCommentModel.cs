using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Models
{
    public class AddCommentModel
    {
        public int ProductId { get; set; }
        public string? Message { get; set; }
        public int? Rating { get; set; }
    }
}
