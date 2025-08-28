using Shop.Application.Exceptions;
using Shop.Application.Interfaces.CQRS.Commands;
using Shop.Application.Interfaces.UOW;
using Shop.Domain.Commons.Constant;
using Shop.Domain.Entitites;

namespace Shop.Application.Features.Commands.Delete
{
    public class DeleteCommandHandler: ICommandHandler<DeleteCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(DeleteCommand command)
        {
            // Lấy shop theo ID
            Shop.Domain.Entitites.Shop? shop = await _unitOfWork.GetRepository<Shop.Domain.Entitites.Shop>().GetByIdAsync(command.Id)
                                              ?? throw new BadRequestException("Shop Id: "+MessageConstant.NotFound);

            _unitOfWork.GetRepository<Shop.Domain.Entitites.Shop>().Delete(shop);

            await _unitOfWork.SaveAsync();

        }
    }
}
