using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Softpark.Application.Interfaces;
using Softpark.Infrastructure.Data;
using Softpark.Infrastructure.Repositories;
using Softpark.Infrastructure.Services;

namespace Softpark.Infrastructure.DependencyInjection
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var databaseSettings = new DatabaseSettings();
            configuration.GetSection("DatabaseSettings").Bind(databaseSettings);

            var jwtSettings = new JwtSettings();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);

            services.AddSingleton(databaseSettings);
            services.AddSingleton(jwtSettings);
            services.AddSingleton(new ConnectionFactory(databaseSettings.ConnectionString));

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ITokenService, JwtTokenService>();

            return services;
        }
    }
}
