# Permissions и роли в User Management Service

## Permissions (Разрешения)

### User Permissions
- `user:read` - Чтение информации о пользователях
- `user:delete` - Удаление пользователей

### User Profile Permissions
- `profile:create` - Создание профиля пользователя
- `profile:read` - Чтение профиля пользователя
- `profile:update` - Обновление профиля пользователя
- `profile:activate` - Активация пользователя (только админ)
- `profile:deactivate` - Деактивация пользователя (только админ)

### Review Permissions
- `review:create` - Создание отзыва
- `review:read` - Чтение отзывов
- `review:update` - Обновление отзыва
- `review:delete` - Удаление отзыва

## Роли и их разрешения

### Registered (Зарегистрированный)
Базовая роль для всех зарегистрированных пользователей.

**Разрешения:**
- `user:read` - Может просматривать пользователей
- `profile:read` - Может просматривать профили
- `review:read` - Может читать отзывы

### Verified (Подтвержденный email)
Пользователь подтвердил email и может создать профиль.

**Разрешения:**
- Все разрешения Registered +
- `profile:create` - Может создать профиль

### Seller (Продавец)
Пользователь создал профиль и может работать с отзывами.

**Разрешения:**
- Все разрешения Verified +
- `profile:update` - Может обновлять свой профиль
- `review:create` - Может создавать отзывы
- `review:update` - Может обновлять свои отзывы
- `review:delete` - Может удалять свои отзывы

### Admin (Администратор)
Полные права на управление пользователями.

**Разрешения:**
- Все разрешения Seller +
- `user:delete` - Может удалять пользователей
- `profile:activate` - Может активировать пользователей
- `profile:deactivate` - Может деактивировать пользователей

## Применение в коде

### В Commands/Queries

```csharp
[Authorize(Permissions = AppPermissions.Review.Read)]
public record GetReviewsQuery(...) : IRequest<...>;

[Authorize(Permissions = AppPermissions.UserProfile.Deactivate)]
public record DeactivateUserProfileCommand(...) : IRequest<...>;
```

### В Controllers

Permissions проверяются автоматически через `AuthorizationBehavior` в MediatR pipeline.

## Политики (Policies)

Дополнительно к permissions используются политики:

- `SelfOrAdmin` - Пользователь может выполнить действие только над своими данными или если он админ
- Применяется в командах Update/Delete для отзывов и профилей

## Миграция базы данных

После изменения permissions необходимо:

1. Создать миграцию:
```bash
dotnet ef migrations add UpdatePermissions --project src/InnoShop.UserManagement.Infrastructure --startup-project src/InnoShop.UserManagement.Api
```

2. Применить миграцию:
```bash
dotnet ef database update --project src/InnoShop.UserManagement.Infrastructure --startup-project src/InnoShop.UserManagement.Api
```

## Добавление новых permissions

1. Добавить константу в `SharedKernel/Security/Permissions/AppPermissions.cs`
2. Добавить Permission в `Domain/UserAggregate/Permission.cs`
3. Обновить `PermissionConfigurations.cs` (HasData)
4. Назначить permission ролям в `RolePermissionConfigurations.cs`
5. Создать и применить миграцию
