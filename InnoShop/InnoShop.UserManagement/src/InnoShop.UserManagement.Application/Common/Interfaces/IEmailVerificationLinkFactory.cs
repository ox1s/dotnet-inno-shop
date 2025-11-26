namespace InnoShop.UserManagement.Application.Common.Interfaces;

public interface IEmailVerificationLinkFactory
{
    string Create(Guid userId, string token);
}