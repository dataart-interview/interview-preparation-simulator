using System.Net.Http.Headers;
using System.Net.Mime;
using Cinema.Domain.Services;
using Cinema.Infrastructure.Caching;
using Cinema.Infrastructure.Configuration;
using Cinema.Infrastructure.Feed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cinema.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCinemaInfrastructure(this IServiceCollection services)
    {
        services.AddOptions<CinemaFeedOptions>()
            .BindConfiguration(CinemaFeedOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<ICinemaFeedClient, CinemaFeedClient>((provider, client) =>
            {
                client.BaseAddress = provider
                    .GetRequiredService<IOptions<CinemaFeedOptions>>()
                    .Value.BaseAddress;
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            })
            .AddStandardResilienceHandler(options =>
            {
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(3);
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(1);
                options.Retry.MaxRetryAttempts = 2;
            });

        services.AddFusionCache();
        services.AddScoped<ISeatMapProvider, CachedSeatMapProvider>();
        return services;
    }
}
