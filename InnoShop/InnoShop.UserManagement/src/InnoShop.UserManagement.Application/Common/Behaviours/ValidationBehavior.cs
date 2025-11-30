using ErrorOr;
using FluentValidation;
using MediatR;

namespace InnoShop.UserManagement.Application.Common.Behaviours;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    // Перехватывает любую команду
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Если валидатора няма - идет дальше
        if (validator is null) return await next(cancellationToken);
        // Если есть, запускает его
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid) return await next(cancellationToken);

        var errors = validationResult.Errors
            .ConvertAll(error => Error.Validation(
                error.PropertyName,
                error.ErrorMessage));

        return (dynamic)errors;
    }
}