using FluentValidation;

namespace Flowtap_Application.Features.Identity.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.OTP).NotEmpty().Length(6);
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8);
    }
}
