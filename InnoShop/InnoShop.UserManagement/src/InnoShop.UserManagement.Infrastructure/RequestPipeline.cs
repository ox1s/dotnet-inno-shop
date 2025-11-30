using InnoShop.UserManagement.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;

namespace InnoShop.UserManagement.Infrastructure;

public static class RequestPipeline
{
    public static IApplicationBuilder AddInfrastructureMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<EventualConsistencyMiddleware>();
        return app;
    }
}