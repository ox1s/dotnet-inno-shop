using ErrorOr;
using FluentValidation;
using MediatR;

namespace InnoShop.ProductManagement.Application.Common.Behaviours;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validator is null) return await next(cancellationToken);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid) return await next(cancellationToken);

        var errors = validationResult.Errors
            .ConvertAll(error => Error.Validation(
                error.PropertyName,
                error.ErrorMessage));

        return (dynamic)errors;
    }
}