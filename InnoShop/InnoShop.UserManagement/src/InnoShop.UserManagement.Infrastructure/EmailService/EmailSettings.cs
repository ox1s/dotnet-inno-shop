namespace InnoShop.UserManagement.Infrastructure.EmailService;

public class EmailSettings
{
    public const string Section = "EmailSettings";

    public string FromEmail { get; set; } = "no-reply@innoshop.com";
    public string FromName { get; set; } = "InnoShop Robot";
}