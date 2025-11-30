namespace InnoShop.SharedKernel.Common.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}