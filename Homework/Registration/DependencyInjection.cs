using GatewayServices.Utilities;

namespace Registration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<UsermanagementConfig>(config => configuration.GetSection(nameof(UsermanagementConfig)).Bind(config));

            return services;
        }
    }
}
