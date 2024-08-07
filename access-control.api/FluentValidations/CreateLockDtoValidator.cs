using access_control.api.Extensions;
using access_control.core.DataTransferObjects;
using FluentValidation;

namespace access_control.api.FluentValidations
{
    public class CreateLockDtoValidator : AbstractValidator<CreateLockDto>
    {
        public CreateLockDtoValidator()
        {
            RuleFor(x  => x.Name)
                .NotEmpty().WithMessage("Please enter a valid lock name and try again")
                .NotNull().WithMessage("Please enter a valid lock name and try again")
                .NoSqlInjection();


            RuleFor(x => x.SerialNumber)
                .NotEmpty().WithMessage("Please enter a valid lock serial number and try again")
                .NotNull().WithMessage("Please enter a valid lock serial number and try again")
                .NoSqlInjection();
        }
    }
}
