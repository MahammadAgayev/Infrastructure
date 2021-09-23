using FluentValidation;
using IdentityServer.Models;

namespace ApiTemplate.Validators
{
    public class PhoneNumberAuthenticateRequestValidator : AbstractValidator<PhoneNumberAuthenticateRequest>
    {
        public PhoneNumberAuthenticateRequestValidator()
        {
            this.RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number cannot be empty");
            this.RuleFor(x => x.Password).NotEmpty().WithMessage("Password cannot be empty");
        }
    }
}