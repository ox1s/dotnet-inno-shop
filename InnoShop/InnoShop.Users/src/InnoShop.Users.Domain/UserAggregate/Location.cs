namespace InnoShop.Users.Domain.UserAggregate;

public record Location(
    string Country,
    string State,
    string? ZipCode,
    string? City,
    string? Street);
