using Shared.Helper;
using Shop.Application.Exceptions;
using Shop.Application.Exceptions.Validators;
using Shop.Application.Interfaces.CQRS.Commands;
using Shop.Application.Interfaces.UOW;
using Shop.Domain.Commons.Constant;

namespace Shop.Application.Features.Commands.Update
{
    public class UpdateCommandHandler: ICommandHandler<UpdateCommand, Shop.Domain.Entitites.Shop>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Domain.Entitites.Shop> HandleAsync(UpdateCommand command)
        {
            // Gọi validator để kiểm tra dữ liệu
            ShopValidate.ValidateUpdate(command);

            // Tìm để cập nhật
            Shop.Domain.Entitites.Shop? shop = await _unitOfWork.GetRepository<Shop.Domain.Entitites.Shop>().GetByIdAsync(command.Id)
               ?? throw new BadRequestException("Shop Id: " + MessageConstant.NotFound);

            if (command.Name != null) shop.Name = command.Name;
            if (command.Description != null) shop.Description = command.Description;
            if (command.WorkingDays != null) shop.WorkingDays = command.WorkingDays;
            if (command.OwnerId != null) shop.OwnerId = command.OwnerId;
            if (command.ImgUrl != null) shop.ImgUrl = command.ImgUrl;

            shop.Status = command.Status;
            shop.UpdatedAt = TimeHelper.GetUtcNow();

            _unitOfWork.GetRepository<Shop.Domain.Entitites.Shop>().Update(shop);
            await _unitOfWork.SaveAsync();
            return shop;

        }
    }
}
