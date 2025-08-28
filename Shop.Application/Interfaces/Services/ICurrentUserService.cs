
namespace Shop.Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Role { get; }
    }
}
