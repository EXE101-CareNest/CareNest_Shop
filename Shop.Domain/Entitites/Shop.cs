using Shop.Domain.Commons;
using Shop.Domain.Commons.Enum;

namespace Shop.Domain.Entitites
{
    public class Shop : BaseEntity
    {
        public required string OwnerId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Status Status { get; set; }
        public string? ImgUrl { get; set; }
        public string? WorkingDays { get; set; }
    }
}
