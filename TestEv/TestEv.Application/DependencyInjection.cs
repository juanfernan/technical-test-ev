using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TestEv.Application.Interfaces;
using TestEv.Application.Services;

namespace TestEv.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IProjectService, ProjectService>();
            services.AddValidatorsFromAssemblyContaining<Validators.CreateProjectValidator>();
            return services;
        }
    }
}
