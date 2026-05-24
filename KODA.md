# KODA.md — Контекст проекта

## Обзор проекта

**Название:** cookbook (web_app_cbx)  
**Тип:** ASP.NET Core веб-приложение  
**Фреймворк:** .NET 9  
**Архитектура:** Blazor Server + HTMX гибридное приложение с REST API

### Назначение

Это веб-приложение для управления рецептами (cookbook) с возможностью:
- Создания, редактирования и просмотра рецептов
- Управления тегами и категориями рецептов
- Загрузки файлов (изображений, документов) к рецептам
- Аутентификации и авторизации пользователей
- Публичной и приватной загрузки файлов

### Технологический стек

| Категория | Технология |
|-----------|------------|
| **Backend** | ASP.NET Core 9, Blazor, HTMX |
| **База данных** | SQLite с EF Core 9 |
| **Идентификация** | ASP.NET Core Identity |
| **Валидация** | FluentValidation |
| **Мониторинг** | OpenTelemetry |
| **Уникальные ID** | Ulid |
| **Форматирование** | CSharpier |

### Структура проекта

```
src/
├── cookbook/                    # Основной проект приложения
│   ├── Constants/               # Константы (роли, статусы)
│   ├── DTOs/                    # Data Transfer Objects
│   │   ├── Auth/               # DTO для аутентификации
│   │   ├── Files/              # DTO для файлов
│   │   ├── Recipes/            # DTO для рецептов
│   │   └── Tags/               # DTO для тегов
│   ├── Domain/
│   │   └── Entities/           # Сущности домена (Recipe, Tag, File, User)
│   ├── Extensions/              # Расширения (HttpContextExtensions)
│   ├── ExceptionHandlers/       # Обработчики исключений
│   ├── Features/                # Функциональные модули (Blazor компоненты)
│   │   ├── Login/              # Страница входа
│   │   ├── EndpointRouting/    # Маршрутизация конечных точек
│   │   ├── Shared/             # Общие компоненты (Layout, NavMenu)
│   │   └── Home.razor          # Главная страница
│   ├── Infrastructure/
│   │   └── db/                 # Контексты базы данных
│   ├── Migrations/             # EF Core миграции
│   ├── Services/               # Бизнес-логика
│   │   ├── AuthService.cs      # Сервис аутентификации
│   │   ├── FilesService.cs     # Сервис работы с файлами
│   │   └── Sorting/            # Сервис сортировки
│   ├── Settings/               # Настройки конфигурации
│   ├── StartupExtensions/      # Расширения для регистрации сервисов
│   │   ├── Auth.cs             # Настройка аутентификации
│   │   ├── Database.cs         # Настройка БД и миграций
│   │   ├── AppServices.cs      # Регистрация бизнес-сервисов
│   │   ├── ErrorHandling.cs    # Настройка обработки ошибок
│   │   └── OpenTelemetry.cs    # Настройка телеметрии
│   ├── Program.cs              # Точка входа
│   ├── appsettings.json        # Конфигурация приложения
│   └── wwwroot/                # Статические файлы
├── .editorconfig               # Настройки форматирования кода
├── .csharpierrc.json           # Настройки CSharpier
├── Directory.Build.props       # Свойства сборки
└── Directory.Packages.props    # Централизованные версии пакетов
```

## Сборка и запуск

### Предварительные требования

- .NET 9 SDK
- Поддерживаемая IDE (Visual Studio 2022, Rider, VS Code)

### Команды

| Действие | Команда |
|----------|---------|
| **Восстановление зависимостей** | `dotnet restore` |
| **Сборка проекта** | `dotnet build` |
| **Запуск приложения** | `dotnet run` |
| **Запуск в режиме разработки** | `dotnet run --launch-profile cookbook` |
| **Создание миграций** | `dotnet ef migrations add <ИмяМиграции>` |
| **Применение миграций** | Автоматически при запуске в Development |
| **Форматирование кода** | `dotnet csharpier .` |

### Конфигурация окружения

Приложение использует следующие секции конфигурации (`appsettings.json`):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=db/cbx.db"
  },
  "Auth": {
    "ExpirationInMinutes": 30,
    "AdminEmail": "admin@admin.ru",
    "AdminPassword": "!Admin12345"
  },
  "Persistence": {
    "Path": "/workspace/persistence"
  },
  "Filestorage": {
    "Path": "/workspace/filestorage"
  }
}
```

### Стандартный пользователь для входа

- **Email:** `admin@admin.ru`
- **Пароль:** `!Admin12345`

## Архитектура и паттерны

### Паттерны проектирования

1. **Clean Architecture** — разделение на слои (Domain, DTOs, Services, Infrastructure)
2. **Repository Pattern** — через EF Core DbContext
3. **Dependency Injection** — встроенный контейнер ASP.NET Core
4. **CQRS-подобная структура** — Features для команд и запросов
5. **Middleware Pipeline** — для обработки запросов и исключений

### Обработка ошибок

Приложение использует:
- `GlobalExceptionHandler` — для глобальных исключений
- `ValidationExceptionHandler` — для валидационных ошибок
- ProblemDetails для стандартизированных ответов об ошибках

### Аутентификация и авторизация

- Cookie-based аутентификация
- Ролевая модель: `Admin`, `Member`
- ASP.NET Core Identity для управления пользователями

### Работа с данными

- EF Core 9 с SQLite
- SnakeCase naming convention для таблиц и колонок
- Migrations для управления схемой БД
- Ulid для генерации уникальных идентификаторов

## Правила разработки

### Форматирование кода

Проект использует **CSharpier** для автоматического форматирования:

```json
{
  "printWidth": 100,
  "useTabs": false,
  "indentSize": 4,
  "endOfLine": "auto"
}
```

### Кодирование соглашения

Из `.editorconfig`:
- **Отступы:** 4 пробела
- **Конечные строки:** CRLF
- **Использование `var`:** только для явных типов
- **Пространства имён:** file-scoped
- **Именованные конвенции:**
  - Интерфейсы: `I` + PascalCase (например, `IService`)
  - Типы: PascalCase
  - Члены: PascalCase
  - Поля: не используются (используются свойства)

### Валидация

- Используется **FluentValidation** для валидации DTO
- Регистрируется через `AddValidatorsFromAssemblyContaining<Program>()`

### Обработка данных

- **Sorting:** Сервис сортировки через `SortMappingProvider`
- **Pagination:** Поддержка пагинации через `PaginationResult<T>`
- **File Upload:** Поддержка загрузки файлов с проверкой MIME-типов

## Тестирование

TODO: Добавить информацию о тестировании, когда тесты будут добавлены в проект.

## Дополнительные ресурсы

### Ключевые файлы для изучения

| Файл | Описание |
|------|----------|
| `Program.cs` | Точка входа, конфигурация middleware pipeline |
| `StartupExtensions/Database.cs` | Настройка EF Core, миграции, seed данных |
| `StartupExtensions/AppServices.cs` | Регистрация бизнес-сервисов |
| `Domain/Entities/` | Сущности домена |
| `Features/` | Blazor компоненты и UI логика |
| `Services/` | Бизнес-логика приложения |

### Пакеты NuGet

| Пакет | Версия | Назначение |
|-------|--------|------------|
| `Microsoft.EntityFrameworkCore.Sqlite` | 9.0.8 | ORM для SQLite |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 9.0.8 | Identity для аутентификации |
| `FluentValidation.DependencyInjectionExtensions` | 12.0.0 | Валидация |
| `OpenTelemetry.*` | 1.12.0 | Мониторинг и телеметрия |
| `EFCore.NamingConventions` | 9.0.0 | SnakeCase для БД |
| `Ulid` | 1.4.1 | Генерация уникальных ID |
| `MimeTypes` | 2.5.2 | Определение MIME-типов |

---

*Файл сгенерирован автоматически на основе анализа кодовой базы.*
