using library_api.Application.Interfaces;
using library_api.Application.Mappings;
using library_api.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace library_api.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddScoped<IBookService, BookService>();
            services.AddScoped<ILoanService, LoanService>();

            return services;
        }
    }
}
