using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Models
{
    public class EditCommentModel
    {
        public Guid CommentId { get; set; }
        public string? Message { get; set; }
        public int? Rating { get; set; }
    }
}
