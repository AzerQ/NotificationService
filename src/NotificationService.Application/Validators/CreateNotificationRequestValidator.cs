using FluentValidation;
using NotificationService.Application.DTOs.Requests;

namespace NotificationService.Application.Validators;

public class CreateNotificationRequestValidator : AbstractValidator<CreateNotificationRequest>
{
    public CreateNotificationRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");

        RuleFor(x => x.Subject)
            .NotEmpty()
            .WithMessage("Subject is required")
            .MaximumLength(500)
            .WithMessage("Subject must not exceed 500 characters");

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage("Body is required");

        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Invalid notification category");

        RuleFor(x => x.Channel)
            .IsInEnum()
            .WithMessage("Invalid notification channel");
    }
}
