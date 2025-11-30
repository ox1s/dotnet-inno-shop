using ErrorOr;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest<ErrorOr<Success>>;
