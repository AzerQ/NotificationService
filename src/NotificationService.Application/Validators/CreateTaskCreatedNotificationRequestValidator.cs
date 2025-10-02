using FluentValidation;
using NotificationService.Application.DTOs.Requests;

namespace NotificationService.Application.Validators;

public class CreateTaskCreatedNotificationRequestValidator : AbstractValidator<CreateTaskCreatedNotificationRequest>
{
    public CreateTaskCreatedNotificationRequestValidator()
    {
        RuleFor(x => x.ExecutorUserId)
            .NotEmpty()
            .WithMessage("ExecutorUserId is required");

        RuleFor(x => x.AuthorName)
            .NotEmpty()
            .WithMessage("AuthorName is required")
            .MaximumLength(200)
            .WithMessage("AuthorName must not exceed 200 characters");

        RuleFor(x => x.TaskSubject)
            .NotEmpty()
            .WithMessage("TaskSubject is required")
            .MaximumLength(500)
            .WithMessage("TaskSubject must not exceed 500 characters");

        RuleFor(x => x.TaskDescription)
            .NotEmpty()
            .WithMessage("TaskDescription is required");

        RuleFor(x => x.TaskType)
            .NotEmpty()
            .WithMessage("TaskType is required");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("DueDate must be in the future");
    }
}
