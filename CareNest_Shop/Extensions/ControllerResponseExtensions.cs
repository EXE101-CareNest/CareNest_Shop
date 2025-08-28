using Microsoft.AspNetCore.Mvc;
using Shop.Application.Common;

namespace CareNest_Shop.Extensions
{
    public static class ControllerResponseExtensions
    {
        public static IActionResult OkResponse<T>(this ControllerBase controller, T data, string? message = null)
        {
            return controller.Ok(ApiResponse<T>.SuccessResponse(data, message));
        }

        public static IActionResult ErrorResponse<T>(this ControllerBase controller, string message)
        {
            return controller.BadRequest(ApiResponse<T>.Failure(message));
        }
    }
}
