using access_control.api.Extensions;
using access_control.core.DataTransferObjects;
using FluentValidation;

namespace access_control.api.FluentValidations
{
    public class RaiseComplaintDtoValidator : AbstractValidator<RaiseComplaintDto>
    {
        public RaiseComplaintDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .EmailAddress().WithMessage("Email must be valid")
                .NoSqlInjection();

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Please provide a valid subject. Field cannot ne empty")
                .NotNull().WithMessage("Please provide a valid subject. Field cannot ne null");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Please provide a valid message body")
                .NotNull().WithMessage("Please provide a valid message body");
        }
    }
}








































































