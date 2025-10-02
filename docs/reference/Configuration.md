# Конфигурация и окружение

## Файлы конфигурации
- `src/NotificationService.Api/appsettings.json` — базовые настройки, строка подключения SQLite и параметры SMTP.
- `src/NotificationService.Api/appsettings.Development.json` — переопределения для разработки.

## Строка подключения к БД
```json
{
  "ConnectionStrings": {
    "Notifications": "Data Source=notifications.db"
  }
}
```

Можно переопределить через переменную окружения:
- ConnectionStrings__Notifications=Data Source=/path/to/notifications.db

## Настройки SMTP (Email)
```json
{
  "Email": {
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 587,
    "EnableSsl": true,
    "UserName": "notification@example.com",
    "Password": "<secret>",
    "FromAddress": "notification@example.com",
    "FromName": "Notification Service"
  }
}
```

Рекомендуется хранить секреты в переменных окружения/Secret Manager:
- Email__UserName
- Email__Password
- Email__FromAddress
- Email__FromName

## Переменные окружения
- ASPNETCORE_ENVIRONMENT (Development/Production)
- ConnectionStrings__Notifications
- Email__SmtpHost, Email__SmtpPort, Email__EnableSsl, Email__UserName, Email__Password, Email__FromAddress, Email__FromName
