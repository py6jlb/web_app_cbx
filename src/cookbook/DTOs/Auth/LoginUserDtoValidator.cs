using FluentValidation;

namespace cookbook.DTOs.Auth;

public sealed class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email обязателен для входа")
            .EmailAddress()
            .WithMessage("Неверный формат email");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Пароль обязателен для входа")
            .MinimumLength(6)
            .WithMessage("Пароль должен содержать минимум 6 символов");
    }
}
