using Shop.Application.Interfaces.CQRS.Commands;
using Shop.Domain.Commons.Enum;

namespace Shop.Application.Features.Commands.Update
{
    public class UpdateCommand: ICommand<Shop.Domain.Entitites.Shop>
    {
        public string Id { get; set; } = string.Empty;
        public string? OwnerId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Status Status { get; set; }
        public string? ImgUrl { get; set; }
        public string? WorkingDays { get; set; }
    }
}
