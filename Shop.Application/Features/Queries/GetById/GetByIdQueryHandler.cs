using Shop.Application.Exceptions;
using Shop.Application.Interfaces.CQRS.Queries;
using Shop.Application.Interfaces.UOW;
using Shop.Domain.Commons.Constant;

namespace Shop.Application.Features.Queries.GetById
{
    public class GetByIdQueryHandler : IQueryHandler<GetByIdQuery, Domain.Entitites.Shop>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Domain.Entitites.Shop> HandleAsync(GetByIdQuery query)
        {
            Domain.Entitites.Shop? shop = await _unitOfWork.GetRepository<Domain.Entitites.Shop>().GetByIdAsync(query.Id);

            if (shop == null)
            {
                throw new BadRequestException(MessageConstant.NotFound);
            }
            return shop;
        }
    }
}
