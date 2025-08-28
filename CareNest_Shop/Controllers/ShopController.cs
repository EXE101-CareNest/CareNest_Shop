using CareNest_Shop.Extensions;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Common;
using Shop.Application.Features.Commands.Create;
using Shop.Application.Features.Commands.Delete;
using Shop.Application.Features.Commands.Update;
using Shop.Application.Features.Queries.GetAllPaging;
using Shop.Application.Features.Queries.GetById;
using Shop.Application.Interfaces.CQRS;
using Shop.Domain.Commons.Constant;
using Shop.Domain.Entitites;

namespace CareNest_Shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly IUseCaseDispatcher _dispatcher;

        public ShopController(IUseCaseDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Hiển thị toàn bộ danh sách cửa hàng hiện có trong hệ thống với phân trang và sắp xếp
        /// </summary>
        /// <param name="pageIndex">trang hiện tại</param>
        /// <param name="pageSize">Số lượng phần tử trong trang</param>
        /// <param name="sortColumn">cột muốn sort: name, updateat,ownerid</param>
        /// <param name="sortDirection">cách sort asc or desc</param>
        /// <returns>Danh sách cửa hàng</returns>
        [HttpGet]
        public async Task<IActionResult> GetPagingShop(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortColumn = null,
            [FromQuery] string? sortDirection = "asc")
        {
            var query = new GetAllPagingQuery()
            {
                Index = pageIndex,
                PageSize = pageSize,
                SortColumn = sortColumn,
                SortDirection = sortDirection
            };
            var result = await _dispatcher.DispatchQueryAsync<GetAllPagingQuery, PageResult<ShopResponse>>(query);
            return this.OkResponse(result, MessageConstant.SuccessGet);
        }

        /// <summary>
        /// Hiển thị chi tiết cửa hàng theo id
        /// </summary>
        /// <param name="id">Id cửa hàng</param>
        /// <returns>chi tiết cửa hàng</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShopById(string id)
        {
            var query = new GetByIdQuery() { Id = id };
            Shop.Domain.Entitites.Shop shop = await _dispatcher.DispatchQueryAsync<GetByIdQuery, Shop.Domain.Entitites.Shop>(query);
            return this.OkResponse(shop, MessageConstant.SuccessGet);
        }

        /// <summary>
        /// tạo mới cửa hàng
        /// </summary>
        /// <param name="command">thông tin cửa hàng</param>
        /// <returns>thông tin cửa hàng mới tạo xog</returns>
        [HttpPost]
        public async Task<IActionResult> CreateShop([FromBody] CreateCommand command)
        {
            Shop.Domain.Entitites.Shop shop = await _dispatcher.DispatchAsync<CreateCommand, Shop.Domain.Entitites.Shop>(command);

            return this.OkResponse(shop, MessageConstant.SuccessCreate);
        }

        /// <summary>
        /// Cập nhật thông tin cửa hàng 
        /// </summary>
        /// <param name="id">Id cửa hàng</param>
        /// <param name="request">các thông tin cần sửa</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShop(string id,[FromBody] UpdateRequest request)
        {

            var command = new UpdateCommand()
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                ImgUrl = request.ImgUrl,
                OwnerId = request.OwnerId,
                Status = request.Status,
                WorkingDays = request.WorkingDays
            };
            Shop.Domain.Entitites.Shop shop = await _dispatcher.DispatchAsync<UpdateCommand, Shop.Domain.Entitites.Shop>(command);

            return this.OkResponse(shop, MessageConstant.SuccessUpdate);
        }

        /// <summary>
        /// xoá cửa hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShop(string id)
        {
            await _dispatcher.DispatchAsync(new DeleteCommand { Id = id });
            return this.OkResponse(MessageConstant.SuccessDelete);
        }
    }
}
