# Руководство разработчика: добавление новых типов уведомлений и каналов

Этот гид описывает, как расширить систему новыми типами уведомлений (payload/шаблоны) и каналами доставки (Email/Sms/Push/...).

## Термины
- Тип уведомления — бизнес-смысл (напр., TaskCreated, TaskCompleted), определяет набор данных и шаблоны.
- Канал — механизм доставки (Email/Sms/Push). Один и тот же тип уведомления может доставляться разными каналами.

## Добавление нового типа уведомления
1. Доменные данные
   - При необходимости расширьте модель `Notification` дополнительными полями либо используйте `Message` и параметризацию шаблонов.
   - Добавьте новый `NotificationTemplate` (Subject/Content/Channel) через репозиторий/миграцию/сидинг.
2. DTO и API
   - При необходимости добавьте новый DTO (например, `TaskCreatedRequestDto`) или используйте универсальный `NotificationRequestDto` с `TemplateName` и `Parameters`.
   - В контроллере можно добавить специализированный endpoint или оставить универсальный `POST /api/notification`.
3. Маппинг и сервисы
   - Если требуется особая логика преобразования параметров в `Message`/`Template.Content`, добавьте сервис подготовки контента (например, `ITemplateRenderer`).
   - Используйте `INotificationCommandService.CreateNotificationAsync(...)` для создания, а `SendNotificationAsync` для отправки.
4. Шаблоны
   - Создайте шаблон(ы) под новый тип: `Name`, `Subject`, `Content`, `Channel`.
   - В `Content` используйте плейсхолдеры для подстановки.

## Добавление нового канала доставки
1. Добавьте интерфейс/реализацию провайдера, если требуется новый канал
   - Реализуйте новый интерфейс или используйте уже существующие: `ISmsProvider`, `IPushNotificationProvider`.
   - Создайте адаптер к внешнему сервису (поставщику SMS/Push).
2. Зарегистрируйте провайдер в DI
   - Пример: `builder.Services.AddScoped<ISmsProvider, TwilioSmsProvider>();`
3. Дополните `NotificationSender`
   - В `NotificationSender.SendAsync` добавьте ветку в switch по новому каналу и вызов соответствующего провайдера.
   - Обновите `NotificationValidator`, если для нового канала требуются новые обязательные поля у `User`/`Notification`.
4. Тестирование
   - Напишите юнит-тесты для провайдера (mock внешнего SDK) и для `NotificationSender` (ветка нового канала).

## Рекомендации по структуре кода
- Соблюдайте разделение слоев: доменные интерфейсы и модели — в Domain; реализации — в Infrastructure.
- Не тяните внешние SDK в Domain/Application.
- DI-композиция — в Api (`Program.cs`).

## Пример мини-процесса (новый канал "WebHook")
1. Domain: добавить интерфейс `IWebhookProvider` (метод `Task<bool> SendAsync(Uri endpoint, string payload)`).
2. Infrastructure: реализовать `WebhookProvider` (HttpClient), зарегистрировать в DI.
3. Application: расширить `NotificationSender` — ветка `NotificationChannel.WebHook` и логика подготовки payload.
4. Tests: покрыть новую ветку `NotificationSender` и `WebhookProvider` моками HttpMessageHandler.

## Обновление схемы БД
- Если требуются новые поля — добавьте их в модели Domain и конфигурации EF в Infrastructure, создайте миграцию и примените её.

## Чек-лист PR
- [ ] Обновлён код Domain/Application/Infrastructure
- [ ] DI зарегистрирован
- [ ] Написаны тесты
- [ ] Обновлена документация (Templates, API примеры)
- [ ] Проверена сборка и линтеры
