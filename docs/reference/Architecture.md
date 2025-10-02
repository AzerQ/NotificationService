# Архитектура проекта NotificationService

Проект организован по многослойной (DDD-подобной) архитектуре с четким разделением ответственности:

- NotificationService.Domain — доменные модели, enum'ы, валидатор, интерфейсы репозиториев и провайдеров.
- NotificationService.Application — DTO, мапперы, сценарии (командный и запросный сервисы), отправитель.
- NotificationService.Infrastructure — EF Core (SQLite), конфигурации сущностей, репозитории, провайдеры (SMTP).
- NotificationService.Api — ASP.NET Core Web API, DI-композиция, Swagger, конфигурация.
- tests/* — модульные тесты для слоев Domain, Application, Infrastructure.

## Взаимодействие слоев

- Api зависит от Application и Infrastructure.
- Application зависит от Domain.
- Infrastructure зависит от Domain (+ Application при необходимости общих контрактов).
- Domain ни от кого не зависит (кроме BCL).

Все зависимости должны быть направлены от верхних слоев к нижним, а между слоями обмен — только через абстракции из Domain.

## Основные компоненты

- Модели: Notification, User, NotificationTemplate; Enums: NotificationChannel, NotificationStatus.
- Валидатор: NotificationValidator (проверяет корректность данных перед отправкой).
- Интерфейсы: INotificationRepository, IUserRepository, ITemplateRepository; IEmailProvider/ISmsProvider/IPushNotificationProvider.
- Application: INotificationCommandService/QueryService, реализация и маппер INotificationMapper.
- Infrastructure: NotificationDbContext, конфигурации сущностей (Fluent API), репозитории/провайдеры, SMTP-обвязка.

## Поток данных (вертикальный срез)

1. Клиент вызывает API (/api/notification...).
2. Controller вызывает Application-сервис (команда/запрос).
3. Application достает доменные объекты через репозитории (Infrastructure), маппит DTO.
4. Для отправки использует NotificationSender, который обращается к провайдерам каналов (например, SMTP).
5. Состояние уведомления сохраняется в БД; клиент получает DTO-ответ.
