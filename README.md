К СОЖАЛЕНИЮ ЗАВЕРШИТЬ ПРОЕКТ НЕ ПОЛУЧИЛОСЬ, не правильно рассчитала свои знания и возможности🥹
из реализованного `User Management`
интеграционные евенты не воркают, не успела их довести до рабочего состояния
все ендпоинты что есть, положены в соответствующие папки `requests`, в swagger с  jwt не получится

user management делала сама, а уже при проектировании product management прибегнула к cursor, чтобы быстро имплементнуть тоже самое, но времени уже не хватило
тесты в user management работают, создавала их в начале, так что некоторые вещи могут быть не понятны, а начиналось так красиво...



<div align="center">

# 📱 InnoShop

**Современная микросервисная платформа электронной коммерции**

[![.NET](https://img.shields.io/badge/.NET%2010-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Aspire](https://img.shields.io/badge/.NET%20Aspire-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/aspire/)
[![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![Postgres](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)](https://www.rabbitmq.com/)

<p align="center">
  <a href="#-архитектура">Архитектура</a> •
  <a href="#-технический-стек">Стек</a> •
  <a href="#-проектирование-домена-ddd">DDD</a> •
  <a href="#-ui--ux-концепт">Интерфейс</a> •
  <a href="#-техническое-задание">ТЗ</a>
</p>
</div>

---

## 📖 О проекте

**InnoShop** - учебный проект магазина б/у гаджетов, построенный на .NET 10, .NET Aspire и принципах DDD/Clean
Architecture.

---

## 🏗 Архитектура

В основе системы лежит оркестратор **.NET Aspire**, управляющий жизненным циклом сервисов. Коммуникация между User
Service и Product Service осуществляется через шину сообщений RabbitMQ, а данные хранятся в изолированных базах
PostgreSQL.

<div align="center">
  <img src="images/architecture.png" alt="Architecture Diagram" width="100%" style="border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);">
</div>

---

## 🚀 Технический стек

| Категория         | Технологии                                             |
|:------------------|:-------------------------------------------------------|
| **Core**          | `ASP.NET Core Web API`, `.NET Aspire`                  |
| **Architecture**  | `Clean Architecture`, `DDD`, `CQRS (MediatR)`          |
| **Database**      | `PostgreSQL` (Database-per-service), `EF Core`         |
| **Messaging**     | `RabbitMQ` (MassTransit)                               |
| **Caching**       | `Redis` (Distributed Cache)                            |
| **Storage**       | `MinIO` (S3 Compatible Object Storage)                 |
| **Communication** | `REST` (Sync), `Message Bus` (Async)                   |
| **Tools**         | `MailKit` + `Mailpit` (SMTP Testing), `Docker Compose` |

---

## 🧩 Проектирование Домена (DDD)

Архитектура приложения строго следует принципам Domain-Driven Design, разделяя логику на агрегаты, сущности и
объекты-значения (Value Objects).

<div align="center">
  <h3>Общая концепция сущностей</h3>
  <img src="images/Domain.png" alt="Domain Structure" width="600">
</div>

<br/>

### 👤 User Management Service

Микросервис, отвечающий за управление пользователями, безопасную аутентификацию и систему отзывов.
<details>
  <summary><b>📸 Показать схему домена User</b></summary>
  <br>
  <div align="center">
    <img src="images/UserManagementDomain.png" alt="User Management Domain" width="800">
  </div>
</details>

### 📦 Product Management Service

Микросервис каталога товаров. Управляет продуктами, категориями, ценами и списками желаемого. Хранит денормализованную
реплику данных о продавце для оптимизации чтения.
<details>
  <summary><b>📸 Показать схему домена Product</b></summary>
  <br>
  <div align="center">
    <img src="images/ProductManagementDomain.png" alt="Product Management Domain" width="800">
  </div>
</details>

---

## 🎨 UI / UX Концепт

Визуализация клиентской части приложения.

<div align="center">
  <img src="images/UI.png" alt="User Interface Mockup" width="800" style="border-radius: 10px;">
</div>

---

## 📄 Техническое задание

Подробное описание функциональных требований, пользовательских сценариев и API контрактов доступно в документации.

<div align="center">

[![TZ Link](https://img.shields.io/badge/ЧИТАТЬ%20ПОЛНОЕ%20ТЗ-TZ.md-2ea44f?style=for-the-badge&logo=markdown)](TZ.md)

</div>
