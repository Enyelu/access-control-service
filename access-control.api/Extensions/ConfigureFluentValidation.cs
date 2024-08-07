using access_control.api.FluentValidations;
using access_control.core.DataTransferObjects;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace access_control.api.Extensions
{
    public static class ConfigureFluentValidation
    {
        public static void InjectFluentValidations(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateLockDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<RaiseComplaintDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<DeletePermissionDtoValidator>();
            services.AddFluentValidationAutoValidation(options =>
            {
                options.DisableDataAnnotationsValidation = true;
            });
            services.AddFluentValidationClientsideAdapters();
        }
    }
}
