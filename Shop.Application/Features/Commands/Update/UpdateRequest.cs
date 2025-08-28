using Shop.Domain.Commons.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Features.Commands.Update
{
    public class UpdateRequest
    {
        public string? OwnerId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Status Status { get; set; }
        public string? ImgUrl { get; set; }
        public string? WorkingDays { get; set; }
    }
}
