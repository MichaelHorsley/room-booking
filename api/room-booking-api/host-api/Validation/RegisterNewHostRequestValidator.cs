using FluentValidation;
using host_api.Requests;

namespace host_api.Validation;

public class RegisterNewHostRequestValidator : AbstractValidator<RegisterNewHostRequest>
{
    public RegisterNewHostRequestValidator()
    {
        RuleFor(request => request.Email).NotEmpty();
        RuleFor(request => request.FirstName).NotEmpty();
        RuleFor(request => request.Surname).NotEmpty();
    }
}