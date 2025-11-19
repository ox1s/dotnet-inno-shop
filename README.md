# 📱 InnoShop — Магазин гаджетов

> Микросервисная платформа электронной коммерции, построенная на .NET 10, .NET Aspire и принципах DDD/Clean Architecture.

<div align="center">

![.NET](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![Postgres](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white)

</div>

## 🏗 Архитектура

Проект реализован с использованием **.NET Aspire** для оркестрации. Система состоит из двух независимых микросервисов, общающихся асинхронно через RabbitMQ.

<div align="center">
  <img src="images/architecture.png" alt="Architecture Diagram" width="800">
</div>

---

## 🎨 UI / UX Концепт

<div align="center">
  <img src="images/UI.png" alt="User Interface Mockup" width="600">
</div>

---

## 🧩 Проектирование Домена (DDD)

<div style="
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: flex-start;
  width: 100%;
  gap: 20px;
  flex-wrap: nowrap;
">
  <div style="flex: 1; min-width: 200px;">
    <h3>Структура агрегатов и сущностей для каждого микросервиса:</h3>
  </div>

  <div style="flex: 1; min-width: 300px; text-align: right;">
    <img src="images/Domain.png" alt="Domain Structure" style="max-width: 100%; height: auto;">
  </div>

</div>

#### 👤 User Management Service

Отвечает за аутентификацию, профили пользователей и рейтинговую систему.

<div align="center">
  <img src="images/UserManagementDomain.png" alt="User Management Domain" width="700">
</div>

#### 📦 Product Management Service

Отвечает за каталог товаров, категории, фильтрацию и избранное. Хранит реплику данных продавца для быстрого чтения.

<div align="center">
  <img src="images/ProductManagementDomain.png" alt="Product Management Domain" width="700">
</div>

---

## 🚀 Технический стек

- **Backend:** ASP.NET Core 10 Web API
- **Orchestration:** .NET Aspire
- **Communication:** REST (Sync), RabbitMQ (Async/Event-Driven)
- **Database:** PostgreSQL (Database-per-service)
- **Caching:** Redis
- **Storage:** MinIO (S3 compatible) for images
- **Mail:** MailKit + Mailpit (SMTP testing)
- **Architecture:** Clean Architecture, CQRS (MediatR), DDD

## 📄 Техническое задание

Полное техническое задание и требования к функционалу описаны в отдельном файле:

👉 **[Читать ТЗ (TZ.md)](TZ.md)**
