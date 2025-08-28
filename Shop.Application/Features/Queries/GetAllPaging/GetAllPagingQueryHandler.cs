using Shop.Application.Common;
using Shop.Application.Interfaces.CQRS.Queries;
using Shop.Application.Interfaces.UOW;

namespace Shop.Application.Features.Queries.GetAllPaging
{
    public class GetAllPagingQueryHandler : IQueryHandler<GetAllPagingQuery, PageResult<ShopResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllPagingQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PageResult<ShopResponse>> HandleAsync(GetAllPagingQuery query)
        {
            var selector = ObjectMapperExtensions.CreateMapExpression<Domain.Entitites.Shop, ShopResponse>();

            var orderByFunc = GetOrderByFunc(query.SortColumn,query.SortDirection);

            IEnumerable<ShopResponse> a = await _unitOfWork.GetRepository<Domain.Entitites.Shop>().FindAsync(
                predicate: null,
                orderBy: orderByFunc,
                selector :  selector,
                pageSize: query.PageSize,
                pageIndex : query.Index);

            return new PageResult<ShopResponse>(a,1,query.PageSize, query.Index);
        }


        private Func<IQueryable<Domain.Entitites.Shop>, IOrderedQueryable<Domain.Entitites.Shop>> GetOrderByFunc(string? sortColumn, string? sortDirection)
        {
            var ascending = string.IsNullOrWhiteSpace(sortDirection) || sortDirection.ToLower() != "desc";

            return sortColumn?.ToLower() switch
            {
                "name" => q => ascending ? q.OrderBy(a => a.Name) : q.OrderByDescending(a => a.Name),
                "ownerid" => q => ascending ? q.OrderBy(a => a.OwnerId) : q.OrderByDescending(a => a.OwnerId),
                "createdate" => q => ascending ? q.OrderBy(a => a.CreatedAt) : q.OrderByDescending(a => a.CreatedAt),
                _ => q => q.OrderBy(a => a.CreatedAt) // fallback nếu không có sortColumn
            };
        }
    }
}
