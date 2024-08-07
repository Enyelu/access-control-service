using access_control.api.FluentValidations;
using FluentValidation;

namespace access_control.api.Extensions
{
    public static class CustomValidatorsExtensions
    {
        public static IRuleBuilderOptions<T, string> NoSqlInjection<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new NoSqlInjectionValidator<T>());
        }
    }
}
