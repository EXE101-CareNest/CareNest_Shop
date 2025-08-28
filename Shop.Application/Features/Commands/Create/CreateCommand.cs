using Shop.Application.Interfaces.CQRS.Commands;
using Shop.Domain.Commons.Enum;
using Shop.Domain.Entitites;
using System.Windows.Input;

namespace Shop.Application.Features.Commands.Create
{
    public class CreateCommand : ICommand<Domain.Entitites.Shop>
    {
        public string? OwnerId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Status Status { get; set; }
        public string? ImgUrl { get; set; }
        public string? WorkingDays { get; set; }
    }
}
