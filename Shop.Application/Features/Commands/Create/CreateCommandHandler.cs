using Shared.Helper;
using Shop.Application.Exceptions.Validators;
using Shop.Application.Interfaces.CQRS.Commands;
using Shop.Application.Interfaces.UOW;

namespace Shop.Application.Features.Commands.Create
{
    public class CreateCommandHandler : ICommandHandler<CreateCommand, Domain.Entitites.Shop>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Domain.Entitites.Shop> HandleAsync(CreateCommand command)
        {
            ShopValidate.ValidateCreate(command);

            Domain.Entitites.Shop shop = new()
            {
                OwnerId = command.OwnerId!,
                Description = command.Description,
                CreatedAt = TimeHelper.GetUtcNow(),
                ImgUrl = command.ImgUrl,
                Status = command.Status,
                WorkingDays = command.WorkingDays,
                Name = command.Name,
                CreatedBy = null
            };
            await _unitOfWork.GetRepository<Domain.Entitites.Shop>().AddAsync(shop);
            await _unitOfWork.SaveAsync();

            return shop;
        }
    }
}
