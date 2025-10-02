# Notification Service API

REST API для отправки уведомлений через различные каналы (Email, SMS, Push).

## Быстрый старт

### Предварительные требования

- .NET 8.0 SDK
- SQLite (встроен)

### Запуск приложения

1. Настройте параметры email в `appsettings.json`:

```json
{
  "EmailProvider": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "noreply@yourdomain.com",
    "FromName": "Notification Service",
    "EnableSsl": true
  }
}
```

2. Запустите приложение:

```bash
cd src/NotificationService.Api
dotnet run
```

3. Откройте Swagger UI в браузере:

```
https://localhost:5001/swagger
```

## API Endpoints

### Users

#### GET /api/users
Получить список всех пользователей.

**Response:**
```json
[
  {
    "id": "guid",
    "name": "string",
    "email": "string",
    "phoneNumber": "string"
  }
]
```

#### GET /api/users/{id}
Получить пользователя по ID.

**Response:**
```json
{
  "id": "guid",
  "name": "string",
  "email": "string",
  "phoneNumber": "string"
}
```

#### POST /api/users
Создать нового пользователя.

**Request:**
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "+1234567890"
}
```

**Response:**
```json
{
  "id": "guid",
  "name": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "+1234567890"
}
```

#### DELETE /api/users/{id}
Удалить пользователя.

**Response:** 204 No Content

### Notifications

#### POST /api/notifications
Отправить уведомление.

**Request:**
```json
{
  "title": "Task Completed",
  "message": "Your task has been completed successfully",
  "recipientId": "guid",
  "channel": 0,
  "templateName": "task-completed"
}
```

**Channel values:**
- 0: Email
- 1: SMS
- 2: Push

**Response:**
```json
{
  "id": "guid",
  "title": "Task Completed",
  "message": "Your task has been completed successfully",
  "createdAt": "2024-01-01T12:00:00Z",
  "recipient": {
    "id": "guid",
    "name": "John Doe",
    "email": "john@example.com"
  },
  "channel": 0,
  "status": 0
}
```

**Status values:**
- 0: Pending
- 1: Sent
- 2: Failed
- 3: Delivered

#### GET /api/notifications/{id}
Получить уведомление по ID.

**Response:**
```json
{
  "id": "guid",
  "title": "string",
  "message": "string",
  "createdAt": "datetime",
  "recipient": { },
  "channel": 0,
  "status": 0
}
```

#### GET /api/notifications/user/{userId}
Получить все уведомления для пользователя.

**Response:**
```json
[
  {
    "id": "guid",
    "title": "string",
    "message": "string",
    "createdAt": "datetime",
    "recipient": { },
    "channel": 0,
    "status": 0
  }
]
```

## Архитектура

Проект использует многослойную архитектуру:

- **Domain Layer** (`NotificationService.Domain`) - Бизнес-модели и интерфейсы
- **Application Layer** (`NotificationService.Application`) - Бизнес-логика и сервисы
- **Infrastructure Layer** (`NotificationService.Infrastructure`) - Реализация провайдеров, репозиториев, доступ к данным
- **API Layer** (`NotificationService.Api`) - REST API контроллеры

### Принципы

- **Dependency Inversion** - все слои зависят от абстракций
- **Repository Pattern** - доступ к данным через репозитории
- **Provider Pattern** - расширяемая система провайдеров уведомлений

## Настройка провайдеров

### Email (SMTP)

Настройте параметры SMTP в `appsettings.json`:

```json
{
  "EmailProvider": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "noreply@yourdomain.com",
    "FromName": "Notification Service",
    "EnableSsl": true
  }
}
```

**Для Gmail:** используйте [App Passwords](https://support.google.com/accounts/answer/185833) вместо основного пароля.

### Для разработки

Используйте локальный SMTP-сервер для тестирования (например, [MailHog](https://github.com/mailhog/MailHog)):

```json
{
  "EmailProvider": {
    "SmtpHost": "localhost",
    "SmtpPort": 1025,
    "Username": "dev@localhost",
    "Password": "dev",
    "FromEmail": "dev@localhost",
    "FromName": "Dev Notification Service",
    "EnableSsl": false
  }
}
```

## База данных

Проект использует SQLite. База данных создается автоматически при первом запуске.

**Connection String:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=notifications.db"
  }
}
```

## Тестирование

Запустите unit-тесты:

```bash
dotnet test
```

Текущее покрытие:
- SmtpEmailProvider (6 тестов)
- UserRepository (7 тестов)

## Расширение функциональности

### Добавление нового провайдера

1. Создайте интерфейс в `NotificationService.Domain.Interfaces`
2. Реализуйте провайдер в `NotificationService.Infrastructure.Providers`
3. Зарегистрируйте в `ServiceCollectionExtensions`
4. Обновите `NotificationService` для использования нового провайдера

### Добавление нового типа уведомления

1. Создайте новый `enum` в `NotificationChannel`
2. Обновите `NotificationService.SendNotificationAsync` для обработки нового канала
3. Создайте соответствующий провайдер

## Лицензия

MIT
