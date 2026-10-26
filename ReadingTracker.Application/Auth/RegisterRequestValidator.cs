using FluentValidation;
using ReadingTracker.Domain.Constants;

namespace ReadingTracker.Application.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(UserConstants.MaxEmailLength)
            .WithMessage($"Email must not exceed {UserConstants.MaxEmailLength} characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(UserConstants.MinPasswordLength)
            .WithMessage($"Password must be at least {UserConstants.MinPasswordLength} characters long.")
            .MaximumLength(UserConstants.MaxPasswordLength)
            .WithMessage($"Password must not exceed {UserConstants.MaxPasswordLength} characters.")
            .Matches(@"[0-9]").When(_ => UserConstants.RequireDigit)
            .WithMessage("Password must contain at least one digit (0-9).");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required.")
            .Must(name => !string.IsNullOrWhiteSpace(name) && name.Trim().Length >= UserConstants.MinDisplayNameLength)
            .WithMessage($"Display name must be at least {UserConstants.MinDisplayNameLength} non-whitespace characters.")
            .MaximumLength(UserConstants.MaxDisplayNameLength)
            .WithMessage($"Display name must not exceed {UserConstants.MaxDisplayNameLength} characters.");
    }
}