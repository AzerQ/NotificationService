# API справочник

Базовый URL: `/api/notification`

## POST `/api/notification`
Создать уведомление.

Request body (NotificationRequestDto):
```
{
  "title": "string",
  "message": "string",
  "recipientId": "uuid",
  "channel": "Email|Sms|Push",
  "templateName": "string",
  "parameters": { "Key": "Value" }
}
```

Responses:
- 201 Created — `NotificationResponseDto` (Location: `/api/notification/{id}`)
- 400 Bad Request — неверные данные

## POST `/api/notification/{id}/send`
Отправить уведомление.

Responses:
- 204 No Content — успешно
- 404 Not Found — уведомление не найдено
- 422 Unprocessable Entity — ошибка в данных уведомления

## GET `/api/notification/{id}`
Получить уведомление по Id.

Responses:
- 200 OK — `NotificationResponseDto`
- 404 Not Found

## GET `/api/notification/by-user/{userId}`
Получить уведомления пользователя.

Responses:
- 200 OK — массив `NotificationResponseDto`

## GET `/api/notification/by-status/{status}`
Получить уведомления по статусу (`Pending|Sent|Failed|Delivered`).

Responses:
- 200 OK — массив `NotificationResponseDto`
- 400 Bad Request — неизвестный статус
