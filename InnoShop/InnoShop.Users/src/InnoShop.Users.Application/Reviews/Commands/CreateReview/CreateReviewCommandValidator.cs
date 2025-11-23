using FluentValidation;

namespace InnoShop.Users.Application.Reviews.Commands.CreateReview;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.Comment)
            .MinimumLength(3)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Comment)
                        || x.Comment is not null);
    }
}
