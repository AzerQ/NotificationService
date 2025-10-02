# Схема базы данных и миграции

## EF Core и SQLite
- Контекст: `NotificationDbContext`
- Таблицы: `Users`, `Templates`, `Notifications`
- Конфигурации: `UserConfiguration`, `NotificationTemplateConfiguration`, `NotificationConfiguration`

## Миграции
- Инициализация:
  - Установлен `dotnet-ef` (в репозитории уже настроено)
  - Создание миграций: `dotnet-ef migrations add <Name> -p src/NotificationService.Infrastructure -s src/NotificationService.Api`
  - Применение: `dotnet-ef database update -p src/NotificationService.Infrastructure -s src/NotificationService.Api`

## Начальные данные (сидинг)
- Выполняется при старте приложения в `Program.cs` через `DbInitializer.InitializeAsync(app.Services)`:
  - Создается тестовый пользователь
  - Добавляются базовые шаблоны `TaskCreated`, `TaskCompleted`
