using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string? Message { get; set; }
        public int? Rating { get; set; }
        public Guid UserId { get; set; }
        public int ProductId { get; set; }
    }
}
