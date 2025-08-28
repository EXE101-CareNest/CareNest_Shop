using Microsoft.AspNetCore.Http;
using Shop.Application.Interfaces.Services;
using System.Security.Claims;

namespace Shop.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string? UserId =>
        _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string? Role =>
        _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
    }
}
