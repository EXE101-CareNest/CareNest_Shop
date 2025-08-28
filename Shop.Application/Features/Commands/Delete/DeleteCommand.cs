using Shop.Application.Interfaces.CQRS.Commands;

namespace Shop.Application.Features.Commands.Delete
{
    public class DeleteCommand: ICommand
    {
        public required string Id { get; set; }
    }
}
