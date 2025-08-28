using Shop.Domain.Commons.Enum;

namespace Shop.Application.Features.Queries.GetAllPaging
{
    public class ShopResponse
    {
        public string? Id { get; set; }
        public string? OwnerId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Status Status { get; set; }
        public string? ImgUrl { get; set; }
        public string? WorkingDays { get; set; }
    }
}
