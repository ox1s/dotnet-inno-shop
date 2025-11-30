using FluentValidation;

namespace InnoShop.UserManagement.Application.Reviews.Commands.UpdateReview;

public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.Comment)
            .MinimumLength(3)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Comment)
                       || x.Comment is not null);
    }
}