using Shop.Application.Interfaces.CQRS.Queries;
namespace Shop.Application.Features.Queries.GetById
{
    public class GetByIdQuery : IQuery<Shop.Domain.Entitites.Shop>
    {
        public required string Id { get; set; }
    }
}
