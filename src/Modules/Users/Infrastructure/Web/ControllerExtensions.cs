using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Orion.SharedKernel;

namespace Users.Infrastructure.Web;

internal static class ControllerExtensions
{
    public static void EnsureIsCurrentUser(this ControllerBase controller, Guid userId)
    {
        var currentUserId = controller.User.FindFirstValue("sub");

        if (currentUserId != userId.ToString())
            throw new AppForbiddenException("Você não pode acessar dados de outro usuário.");
    }
}
