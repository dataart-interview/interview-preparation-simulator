using Microsoft.Extensions.DependencyInjection;

namespace Cinema.Api;

public static class DependencyInjection
{
    public static IMvcBuilder AddCinemaApi(this IServiceCollection services) =>
        services.AddControllers().AddApplicationPart(typeof(DependencyInjection).Assembly);
}
