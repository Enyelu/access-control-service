using access_control.core.DataTransferObjects;
using FluentValidation;

namespace access_control.api.FluentValidations
{
    public class DeletePermissionDtoValidator : AbstractValidator<DeletePermissionDto>
    {
        public DeletePermissionDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Please enter a valid Id")
                .NotNull().WithMessage("Please enter a valid Id");


            RuleFor(x => x.LockId)
                .NotEmpty().WithMessage("Please enter a valid lock ID and try again")
                .NotNull().WithMessage("Please enter a valid lock ID and try again");
        }
    }
}
