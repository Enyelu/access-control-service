using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace access_control.api.FluentValidations
{
    public class NoSqlInjectionValidator<T> : PropertyValidator<T, string>
    {
        public override string Name => "NoSqlInjectionValidator";

        public override bool IsValid(ValidationContext<T> context, string value)
        {
            if (string.IsNullOrEmpty(value))
                return true;

            // Define a regex pattern for detecting basic SQL injection patterns
            var regex = new Regex(@"('|--|;|\b(SELECT|INSERT|DELETE|UPDATE|DROP|ALTER|TRUNCATE)\b)", RegexOptions.IgnoreCase);
            return !regex.IsMatch(value);
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
            => "{PropertyName} contains potentially dangerous content.";
    }
}
