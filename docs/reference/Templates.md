# Шаблоны уведомлений

## Модель шаблона
- Name — имя шаблона (уникально в связке с каналом)
- Subject — тема письма (для email)
- Content — содержимое (HTML для email)
- Channel — канал доставки (Email/Sms/Push)

## Переменные шаблонов
Шаблоны могут содержать плейсхолдеры в стиле `{{VariableName}}`. Подстановкой занимается вызывающая логика до отправки (в текущей версии подставляется напрямую текст `Message` из уведомления, либо берется `Template.Content`).

Примеры (см. README):
- TaskCreated: AuthorName, TaskSubject, TaskDescription, TaskType, DueDate, CreatedDate
- TaskCompleted: ExecutorName, TaskSubject, TaskDescription, TaskType, CompletionDate, CreatedDate

## Рекомендации по разработке
- Держать контент HTML простым и кросс-почтовым
- Избегать тяжелых таблиц/стилей, проверять на мобильных
- Все внешние ссылки должны быть https
