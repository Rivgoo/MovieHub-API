# 🎬 MovieHub API

[![.NET](https://img.shields.io/badge/.NET-9-blueviolet.svg?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9-blueviolet.svg?style=flat-square&logo=aspnetcore)](https://dotnet.microsoft.com/apps/aspnet)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-8-lightgrey.svg?style=flat-square&logo=entityframework)](https://docs.microsoft.com/ef/core/)
[![MySQL](https://img.shields.io/badge/MySQL-8.x-orange.svg?style=flat-square&logo=mysql)](https://www.mysql.com/)
[![JWT](https://img.shields.io/badge/JWT-Authentication-green.svg?style=flat-square&logo=jsonwebtokens)](https://jwt.io/)
[![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D.svg?style=flat-square&logo=swagger)](https://swagger.io/)

**MovieHub API** - це потужний RESTful сервіс, розроблений на платформі .NET 9 та ASP.NET Core. API відповідає за комплексне управління даними, пов'язаними з фільмами, акторами, жанрами, сеансами, кінозалами, користувачами та їх бронюваннями, а також забезпечує безпечну автентифікацію та авторизацію.

**Проект був розроблений у межах виробничої практики від SoftServe у 2025 р.**

## ✨ Ключові Можливості

*   **🛡️ Багатошарова Архітектура:** Дотримання принципів Clean Architecture для чіткого розділення відповідальностей.
*   **🔐 Безпека:** Автентифікація на основі JWT та авторизація на основі ролей (Admin, Customer).
*   **🗄️ Управління Даними:** Повний набір CRUD-операцій для всіх ключових сутностей.
*   **🔍 Розширена Фільтрація:** Гнучка система фільтрації, сортування та пагінації даних для API-клієнтів.
*   **🖼️ Робота з Файлами:** Завантаження, зберігання та видалення постерів фільмів та фотографій акторів.
*   **🕰️ Фонова Обробка:** Автоматичне підтвердження бронювань через фоновий сервіс.
*   **🌱 Сідінг Даних:** Можливість заповнення бази даних тестовими даними для розробки та тестування.
*   **🔢 Версіонування API:** Підтримка версії V1 для майбутнього розвитку без порушення зворотної сумісності.
*   **📖 Документація API:** Автоматична генерація інтерактивної документації за допомогою Swagger (OpenAPI).

## 🚀 Початок роботи

### Передумови
*   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
*   [MySQL Server](https://dev.mysql.com/downloads/mysql/) (версія 8.x рекомендується)
*   Будь-яка IDE для .NET розробки (Visual Studio, VS Code, Rider)

### Встановлення та Запуск
1.  **Клонуйте репозиторій:**
    ```bash
    git clone https://github.com/Rivgoo/MovieHub-API.git
    cd MovieHub-API
    ```
2.  **Налаштування бази даних:**
    *   Переконайтесь, що ваш сервер MySQL запущений.
    *   Відкрийте файл `src/Web.API/appsettings.Development.json`.
    *   Знайдіть секцію `ConnectionStrings` та оновіть `DataBaseConnection` актуальними даними для підключення до вашої MySQL бази даних:
        ```json
        "ConnectionStrings": {
          "DataBaseConnection": "Server=localhost;Port=3306;Database=moviewposter_dev;Uid=your_mysql_user;Pwd=your_mysql_password;"
        }
        ```
        Замініть `moviewposter_dev` на бажане ім'я бази даних, `your_mysql_user` та `your_mysql_password` на ваші облікові дані.
3.  **Застосування міграцій:**
    *   API налаштоване на автоматичне застосування міграцій при старті (якщо `ApplyMigrations` в `appsettings.json` встановлено в `true`). При першому запуску база даних та її схема будуть створені.
    *   Альтернативно, ви можете застосувати міграції вручну через .NET CLI з кореневої директорії проекту `Infrastructure` або `Web.API` (якщо він встановлений як startup-project):
        ```bash
        dotnet ef database update --project ../Infrastructure --startup-project ../Web.API
        ```
4.  **Налаштування початкового адміністратора (опціонально):**
    *   У файлі `src/Web.API/appsettings.Development.json` (або `appsettings.json`) можна налаштувати дані для початкового адміністратора в секції `InitialAdmin`. Якщо користувача з таким email не існує, він буде створений при першому запуску.
        ```json
        "InitialAdmin": {
          "FirstName": "Admin",
          "LastName": "Admin",
          "Email": "admin@example.com",
          "Password": "YourSecurePassword_123",
          "PhoneNumber": "+380000000000"
        }
        ```
5.  **Запуск API:**
    *   З Visual Studio: Натисніть кнопку "Play" (зазвичай з профілем "Swagger" або "HTTP").
    *   Через .NET CLI з директорії `src/Web.API`:
        ```bash
        dotnet run
        ```
6.  **Доступ до API та Swagger:**
    *   API буде доступне за адресою, вказаною в `launchSettings.json` (зазвичай `http://localhost:5000` або `https://localhost:5001`).
    *   Інтерактивна документація Swagger буде доступна за адресою `/swagger` (наприклад, `http://localhost:5000/swagger`).

## 🛠️ Конфігурація

Основні налаштування проекту знаходяться у файлах:
*   `src/Web.API/appsettings.json` (загальні налаштування)
*   `src/Web.API/appsettings.Development.json` (налаштування для середовища розробки, перекривають загальні)

Ключові параметри для конфігурації:
*   `ConnectionStrings:DataBaseConnection`: Рядок підключення до MySQL.
*   `ApplyMigrations`: (true/false) Чи застосовувати міграції БД автоматично при старті.
*   `JWT`: Налаштування для JWT токенів (секрет, емітент, аудиторія, час життя).
*   `InitialAdmin`: Дані для створення початкового адміністратора.
*   `PublicDataFolder`, `ContentPosterPath`, etc.: Шляхи та обмеження для завантажуваних файлів.
*   `AllowedOrigins`: Список дозволених джерел для CORS.
*   `UseSwagger`: (true/false) Чи вмикати Swagger UI.
*   `FakeDataSeed:Enabled`: (true/false) Чи дозволяти сідінг даних через API ендпоінт.

## 📄 Документація API (Swagger)

Після запуску проекту, інтерактивна документація API (Swagger UI) буде доступна за маршрутом `/swagger`. Вона дозволяє переглядати всі доступні ендпоінти, їхні параметри, моделі запитів/відповідей та тестувати API безпосередньо з браузера.

---
*Детальнішу технічну документацію щодо архітектури, патернів та структури проекту дивіться нижче.*
---

# 🌐 MovieHub API - Технічна Документація

Цей документ описує архітектуру, технології, патерни та ключові аспекти API для проекту MovieHub. API розроблено на платформі .NET 9 та ASP.NET Core, призначене для взаємодії з фронтенд-додатком та управління даними кінотеатру.

## 📜 Зміст

- [🎬 MovieHub API](#-moviehub-api)
  - [✨ Ключові Можливості](#-ключові-можливості)
  - [🚀 Початок роботи](#-початок-роботи)
    - [Передумови](#передумови)
    - [Встановлення та Запуск](#встановлення-та-запуск)
  - [🛠️ Конфігурація](#️-конфігурація)
  - [📄 Документація API (Swagger)](#-документація-api-swagger)
  - [*Детальнішу технічну документацію щодо архітектури, патернів та структури проекту дивіться нижче.*](#детальнішу-технічну-документацію-щодо-архітектури-патернів-та-структури-проекту-дивіться-нижче)
- [🌐 MovieHub API - Технічна Документація](#-moviehub-api---технічна-документація)
  - [📜 Зміст](#-зміст)
  - [🚀 Загальний Огляд](#-загальний-огляд)
  - [🏛️ Архітектура](#️-архітектура)
    - [Шари Додатку](#шари-додатку)
    - [Взаємодія між шарами](#взаємодія-між-шарами)
  - [🛠️ Технології та Бібліотеки](#️-технології-та-бібліотеки)
  - [🧩 Патерни та Підходи](#-патерни-та-підходи)
  - [📁 Структура Проекту API](#-структура-проекту-api)
    - [Domain](#domain)
    - [Application](#application)
    - [Infrastructure](#infrastructure)
    - [Web.API](#webapi)
  - [🔌 Ключові Ендпоінти API](#-ключові-ендпоінти-api)
    - [Актори (Actors)](#актори-actors)
    - [Бронювання (Bookings)](#бронювання-bookings)
    - [Кінозали (CinemaHalls)](#кінозали-cinemahalls)
    - [Контент (Contents)](#контент-contents)
    - [Жанри (Genres)](#жанри-genres)
    - [Сеанси (Sessions)](#сеанси-sessions)
    - [Користувачі (Users)](#користувачі-users)
    - [Автентифікація (Auth)](#автентифікація-auth)
  - [🔑 Автентифікація та Авторизація](#-автентифікація-та-авторизація)
  - [🚦 Обробка Помилок](#-обробка-помилок)
  - [⚙️ Конфігурація](#️-конфігурація-1)
  - [🖼️ Робота з Файлами](#️-робота-з-файлами)
  - [🗄️ База Даних](#️-база-даних)
  - [🌱 Сідінг Даних (Data Seeding)](#-сідінг-даних-data-seeding)

## 🚀 Загальний Огляд

MovieHub API – це RESTful сервіс, побудований на **ASP.NET Core (.NET 9)**, який слугує бекендом для веб-додатку MovieHub. Він відповідає за управління всіма даними, пов'язаними з фільмами, акторами, жанрами, сеансами, кінозалами, користувачами та їх бронюваннями.

**Основні можливості API:**
*   CRUD-операції для всіх основних сутностей.
*   Автентифікація користувачів (клієнтів та адміністраторів) за допомогою JWT.
*   Авторизація на основі ролей.
*   Розширені можливості фільтрації, сортування та пагінації даних.
*   Завантаження та зберігання файлів (постери, фото акторів).
*   Автоматичне оновлення статусів бронювань та сеансів (через фонові служби).
*   Можливість сідінгу бази даних тестовими даними.

API використовує **версіонування** (наразі версія V1), що дозволяє еволюціонувати без порушення сумісності з існуючими клієнтами.

## 🏛️ Архітектура

Проект дотримується принципів багатошарової архітектури, що сприяє розділенню відповідальностей, тестуванню та масштабуванню. Архітектура — **Clean Architecture**.

### Шари Додатку

1.  **🌐 `Web.API` (Presentation Layer):**
    *   Відповідає за обробку HTTP-запитів та формування HTTP-відповідей.
    *   Містить контролери API, моделі запитів (Requests) та відповідей (Responses), налаштування Swagger.
    *   Виконує валідацію вхідних даних.
    *   Делегує обробку бізнес-логіки до шару `Application`.

2.  **🧩 `Application` (Application/Use Case Layer):**
    *   Містить бізнес-логіку додатку.
    *   Визначає інтерфейси для сервісів (`IEntityService`, `IActorService` тощо) та репозиторіїв (`IRepository`, `IActorRepository`).
    *   Містить реалізації сервісів, DTO (Data Transfer Objects) для передачі даних між шарами.
    *   Координує роботу репозиторіїв через `IUnitOfWork`.
    *   Використовує `Result` патерн для обробки успішних операцій та помилок.
    *   Не залежить від інфраструктурних деталей (наприклад, конкретної СУБД).

3.  **🧱 `Domain` (Domain/Entities Layer):**
    *   Містить основні бізнес-сутності (Entities: `Actor`, `Content`, `User` тощо) та їх логіку.
    *   Визначає базові абстракції для сутностей (`IBaseEntity`, `IAuditableEntity`).
    *   Містить перелічення (`Enums`: `BookingStatus`, `SessionStatus`).
    *   Не має залежностей від інших шарів. Це ядро системи.

4.  **🛠️ `Infrastructure` (Infrastructure Layer):**
    *   Реалізує інтерфейси, визначені в `Application` шарі, для взаємодії із зовнішніми системами.
    *   Містить реалізації репозиторіїв (`ActorRepository`, `ContentRepository`) з використанням **Entity Framework Core**.
    *   Реалізація `UnitOfWork` (`UnitOfWork.cs`).
    *   `CoreDbContext` - контекст бази даних EF Core.
    *   Міграції бази даних.
    *   Сервіси для роботи з файловою системою (`LocalContentFileStorageService`).
    *   Реалізації селекторів та сортерів для фільтрації даних.

### Взаємодія між шарами

*   **Залежності спрямовані всередину:** `Web.API` -> `Application` -> `Domain`. `Infrastructure` також залежить від `Application` (для реалізації інтерфейсів) та `Domain` (для роботи з сутностями). `Domain` не залежить ні від кого.
*   **Контролери (`Web.API`)** отримують запити, мапують їх на команди або запити `Application` шару за допомогою DTO та AutoMapper.
*   **Сервіси (`Application`)** виконують бізнес-логіку, використовуючи репозиторії для доступу до даних та `UnitOfWork` для управління транзакціями.
*   **Репозиторії (`Infrastructure`)** реалізують доступ до даних, використовуючи EF Core та `CoreDbContext`.
*   **Сутності (`Domain`)** використовуються на всіх шарах для представлення даних.

## 🛠️ Технології та Бібліотеки

*   **⚙️ .NET 9 / ASP.NET Core 9:** Основна платформа для розробки API.
*   **🗄️ Entity Framework Core (EF Core):** ORM для взаємодії з базою даних (MySQL).
    *   **Code First:** Схема бази даних генерується на основі моделей C#.
    *   **Міграції:** Для управління змінами схеми бази даних.
    *   **Pomelo.EntityFrameworkCore.MySql:** Провайдер для MySQL.
    *   **EFCore.NamingConventions:** Для автоматичного перетворення імен таблиць та стовпців у `snake_case`.
*   **🔐 ASP.NET Core Identity:** Для управління користувачами, ролями, автентифікацією та авторизацією.
*   **🔑 JSON Web Tokens (JWT):** Для захисту API ендпоінтів.
    *   `System.IdentityModel.Tokens.Jwt` та `Microsoft.AspNetCore.Authentication.JwtBearer`.
*   **🔄 AutoMapper:** Для автоматичного мапування об'єктів (наприклад, між сутностями та DTO).
*   **🔢 Asp.Versioning.Mvc.ApiExplorer:** Для версіонування API.
*   **📖 Swashbuckle.AspNetCore (Swagger):** Для генерації інтерактивної документації API.
    *   `Unchase.Swashbuckle.AspNetCore.Extensions`: Для покращеного відображення enum'ів.
*   **🧪 Bogus:** Бібліотека для генерації фейкових даних (використовується в `SeedService`).
*   **🔗 LinqKit:** Для розширення можливостей LINQ, зокрема для побудови динамічних предикатів (використовується в сортерах).
*   **📦 NuGet Пакети:** Проект використовує стандартний набір пакетів для ASP.NET Core, EF Core, Identity, JWT тощо.

## 🧩 Патерни та Підходи

*   **🛡️ Clean Architecture (або подібний):** Розділення відповідальностей на шари (Domain, Application, Infrastructure, Presentation).
*   **🧱 Repository Pattern:** Абстрагує доступ до даних.
    *   Інтерфейси: `IRepository`, `IEntityOperations<TEntity, TId>`, специфічні інтерфейси (напр., `IActorRepository`).
    *   Реалізації: `OperationsRepository<TEntity, TId>`, конкретні репозиторії (напр., `ActorRepository`).
*   **💾 Unit of Work Pattern:** Координує збереження змін в рамках однієї транзакції.
    *   Інтерфейс: `IUnitOfWork`.
    *   Реалізація: `UnitOfWork.cs`.
*   **⚙️ Service Layer Pattern:** Інкапсулює бізнес-логіку.
    *   Інтерфейси: `IEntityService<TEntity, TId>`, специфічні інтерфейси (напр., `IActorService`).
    *   Реалізації: `BaseEntityService<TEntity, TId, TRepository>`, конкретні сервіси (напр., `ActorService`).
*   **💉 Dependency Injection (DI):** Широко використовується для управління залежностями. Конфігурація в `Program.cs` та файлах `Dependency.cs` кожного шару.
*   **📝 Options Pattern:** Для управління конфігураційними параметрами (`JwtOptions`, `InitialAdminOptions`).
*   **🏁 Result Pattern:** Для обробки результатів операцій та помилок.
    *   Класи: `Result`, `Result<TValue>`, `Error`, `ErrorType`.
    *   Розширення: `ResultExtensions` для зручного мапування на HTTP-результати.
*   **🔄 Data Transfer Objects (DTO):** Використовуються для передачі даних між шарами та API-клієнтом, запобігаючи витоку деталей сутностей домену.
    *   Розміщені в `Application` шарі (`Application/[EntityName]/Dtos`) та в `Web.API` (`Web.API/Controllers/V1/[EntityName]/Requests` та `Responses`).
*   **🔍 Filters, Selectors, Sorters:** Гнучка система для побудови динамічних запитів до бази даних.
    *   `IFilterService`, `ISelector`, `ISorter` в `Application/Filters/Abstractions`.
    *   Реалізації в `Infrastructure/Filters/Selectors` та `Infrastructure/Filters/Sorters`.
    *   Дозволяє клієнту вказувати параметри фільтрації, сортування та пагінації.
*   **🕰️ Background Services:** `BookingConfirmationService` для асинхронних фонових завдань (наприклад, автоматичне підтвердження бронювань).
*   **🚦 Fluent Validation (через атрибути):** Атрибути валідації (`[Required]`, `[MaxLength]`, `[EmailAddress]`) використовуються на DTO запитів (`Requests`) для валідації вхідних даних на рівні контролерів.
*   **🌐 RESTful API Design:** Дотримання принципів REST для побудови API (використання HTTP-методів, статус-кодів, URI).

## 📁 Структура Проекту API

Проект API організований за шарами для чіткого розділення відповідальностей:

### Domain
*   **`Abstractions`**: Базові інтерфейси та класи для сутностей (`IBaseEntity`, `IAuditableEntity`, `BaseEntity<TId>`).
*   **`Entities`**: Визначення основних доменних моделей (напр., `Actor.cs`, `Content.cs`, `User.cs`). Це POCO-класи, що представляють структуру даних.
*   **`Enums`**: Перелічення, що використовуються в домені (напр., `BookingStatus.cs`, `SessionStatus.cs`).
*   **`RoleList.cs`**: Статичний клас з константами для імен ролей.

### Application
*   **`Abstractions`**:
    *   `Repositories`: Інтерфейси для репозиторіїв (`IAddOperations`, `IDeleteOperations`, `IEntityOperations`, `IRepository` тощо).
    *   `Services`: Інтерфейси для сервісів (`IEntityService`).
    *   `IUnitOfWork.cs`: Інтерфейс для Unit of Work.
*   **`[EntityName]` (напр., `Actors`, `Bookings`)**: Директорії для кожної бізнес-сутності.
    *   `Abstractions`: Специфічні інтерфейси для репозиторіїв та сервісів цієї сутності (напр., `IActorRepository`, `IActorService`).
    *   `Dtos`: Data Transfer Objects для передачі даних (напр., `ActorDto.cs`).
    *   `[EntityName]Service.cs`: Реалізація бізнес-логіки для сутності.
    *   `[EntityName]Errors.cs`: Статичний клас з визначеннями специфічних помилок для сутності.
    *   `[EntityName]Filter.cs`: Клас, що описує параметри фільтрації для сутності.
*   **`Files`**: Абстракції для роботи з файлами (`IContentFileStorageService.cs`).
*   **`Filters`**: Абстракції та базові класи для фільтрації, сортування та пагінації (`IFilter`, `ISelector`, `ISorter`, `PaginatedList.cs`).
*   **`Results`**: Класи для реалізації Result Pattern (`Result.cs`, `Error.cs`, `ErrorType.cs`).
*   **`Roles/Dtos`**: DTO для ролей.
*   **`Seeds`**: Логіка для заповнення бази даних тестовими даними (`SeedService.cs`, `ISeedService.cs`).
*   **`Users/Models`**: Моделі, специфічні для користувачів (`RegistrationUserModel.cs`, `AuthenticationResult.cs`).
*   **`Utilities`**: Допоміжні класи (напр., `StringUtilities.cs`, `Guard.cs`).
*   **`Dependency.cs`**: Реєстрація залежностей для шару `Application`.

### Infrastructure
*   **`Abstractions`**: Базові класи для реалізації репозиторіїв (`BaseOperationsRepository.cs`, `OperationsRepository.cs`).
*   **`Core`**:
    *   `CoreDbContext.cs`: Головний контекст бази даних Entity Framework Core.
    *   `UnitOfWork.cs`: Реалізація `IUnitOfWork`.
*   **`Files`**:
    *   `LocalContentFileStorageService.cs`: Реалізація сервісу для збереження файлів на локальному диску.
*   **`Filters`**:
    *   `Selectors`: Реалізації `ISelector` для мапування сутностей на DTO (напр., `ActorSelector.cs`).
    *   `Sorters`: Реалізації `ISorter` для застосування фільтрів та сортування (напр., `ActorSorter.cs`).
*   **`Migrations`**: Згенеровані EF Core міграції для управління схемою бази даних.
*   **`Repositories`**: Реалізації інтерфейсів репозиторіїв з `Application` шару (напр., `ActorRepository.cs`).
*   **`Dependency.cs`**: Реєстрація залежностей для шару `Infrastructure`.

### Web.API
*   **`Controllers/V1/[EntityName]`**: Контролери API, згруповані за сутностями. Кожен контролер може містити піддиректорії `Requests` та `Responses` для своїх DTO.
*   **`Core`**:
    *   `BaseResponses`: Загальні моделі відповідей (`CreatedResponse.cs`, `ExistsResponse.cs`).
    *   `Jwt`: Класи для роботи з JWT (`JwtAuthentication.cs`, `JwtOptions.cs`).
    *   `Options`: Класи для конфігураційних опцій (`InitialAdminOptions.cs`).
    *   `ApiController.cs`, `EntityApiController.cs`: Базові класи для контролерів.
    *   `ResultExtensions.cs`: Методи розширення для перетворення `Result` об'єктів на `IActionResult`.
*   **`Extensions`**: Методи розширення для `IServiceCollection` та `WebApplication` для налаштування сервісів та конвеєра запитів (`ServiceCollectionExtensions.cs`, `ApplicationBuilderExtensions.cs`, `AppInitializer.cs`).
*   **`Properties/launchSettings.json`**: Налаштування запуску проекту.
*   **`appsettings.json`, `appsettings.Development.json`**: Файли конфігурації.
*   **`ConfigureSwaggerOptions.cs`**: Клас для налаштування Swagger.
*   **`Program.cs`**: Головний файл для конфігурації та запуску додатку.

## 🔌 Ключові Ендпоінти API

API надає стандартні RESTful ендпоінти для управління основними сутностями. Більшість ендпоінтів захищені та вимагають автентифікації (JWT) та авторизації (ролі).

*   `[AllowAnonymous]` - Дозволяє анонімний доступ.
*   `[Authorize(Roles = RoleList.Admin)]` - Доступ лише для адміністраторів.
*   `[Authorize(Roles = RoleList.Customer)]` - Доступ лише для клієнтів.
*   `[Authorize]` - Доступ для будь-якого автентифікованого користувача.

### Актори (Actors)
*   `GET /api/v1/actors/filter`: Отримання списку акторів з фільтрацією, сортуванням та пагінацією (доступно анонімно).
*   `GET /api/v1/actors/{id}`: Отримання актора за ID (доступно анонімно).
*   `GET /api/v1/actors/{id}/exists`: Перевірка існування актора (вимагає авторизації).
*   `GET /api/v1/actors/{id}/in-content/{contentId}`: Отримання інформації про роль актора у конкретному контенті (доступно анонімно).
*   `POST /api/v1/actors`: Створення нового актора (лише Admin).
*   `PUT /api/v1/actors/{id}`: Оновлення актора (лише Admin).
*   `DELETE /api/v1/actors/{id}`: Видалення актора (лише Admin).
*   `POST /api/v1/actors/{id}/photo`: Завантаження/оновлення фото актора (лише Admin).
*   `DELETE /api/v1/actors/{id}/photo`: Видалення фото актора (лише Admin).

### Бронювання (Bookings)
*   `GET /api/v1/bookings/filter`: Отримання списку бронювань з фільтрацією (Admin або власник бронювань).
*   `GET /api/v1/bookings/{id}`: Отримання бронювання за ID (Admin або власник).
*   `GET /api/v1/bookings/{id}/exists`: Перевірка існування бронювання (вимагає авторизації).
*   `GET /api/v1/bookings/sessions/{sessionId}/seats/{rowNumber}/{seatNumber}/is-booked`: Перевірка, чи заброньоване місце на сеансі (доступно анонімно).
*   `POST /api/v1/bookings`: Створення нового бронювання (лише Customer).
*   `PUT /api/v1/bookings/{id}/cancel`: Скасування бронювання (Admin або власник).
*   `DELETE /api/v1/bookings/{id}`: Видалення бронювання (лише Admin).

### Кінозали (CinemaHalls)
*   `GET /api/v1/cinema-halls/filter`: Отримання списку кінозалів з фільтрацією (доступно анонімно).
*   `GET /api/v1/cinema-halls`: Отримання всіх кінозалів (лише Admin).
*   `GET /api/v1/cinema-halls/{id}`: Отримання кінозалу за ID (доступно анонімно).
*   `GET /api/v1/cinema-halls/{id}/exists`: Перевірка існування кінозалу (вимагає авторизації).
*   `POST /api/v1/cinema-halls`: Створення нового кінозалу (лише Admin).
*   `PUT /api/v1/cinema-halls/{id}`: Оновлення кінозалу (лише Admin).
*   `DELETE /api/v1/cinema-halls/{id}`: Видалення кінозалу (лише Admin).

### Контент (Contents)
*   `GET /api/v1/contents/filter`: Отримання списку контенту (фільмів) з фільтрацією (доступно анонімно, з опцією `isFavorited` для авторизованих).
*   `GET /api/v1/contents`: Отримання всього контенту (лише Admin).
*   `GET /api/v1/contents/{id}`: Отримання контенту за ID (доступно анонімно).
*   `GET /api/v1/contents/{id}/exists`: Перевірка існування контенту (доступно анонімно).
*   `POST /api/v1/contents`: Створення нового контенту (лише Admin).
*   `PUT /api/v1/contents/{id}`: Оновлення контенту (лише Admin).
*   `DELETE /api/v1/contents/{id}`: Видалення контенту (лише Admin).
*   `POST /api/v1/contents/{id}/poster`: Завантаження/оновлення постера (лише Admin).
*   `DELETE /api/v1/contents/{id}/poster`: Видалення постера (лише Admin).
*   `POST /api/v1/contents/{id}/banner`: Завантаження/оновлення банера (лише Admin).
*   `DELETE /api/v1/contents/{id}/banner`: Видалення банера (лише Admin).
*   `POST /api/v1/contents/{id}/genres/{genreId}`: Додавання жанру до контенту (лише Admin).
*   `DELETE /api/v1/contents/{id}/genres/{genreId}`: Видалення жанру з контенту (лише Admin).
*   `POST /api/v1/contents/{id}/actors`: Додавання актора до контенту з роллю (лише Admin).
*   `DELETE /api/v1/contents/{id}/actors/{actorId}`: Видалення актора з контенту (лише Admin).

### Жанри (Genres)
*   `GET /api/v1/genres`: Отримання всіх жанрів (доступно анонімно).
*   `GET /api/v1/genres/{id}`: Отримання жанру за ID (доступно анонімно).
*   `GET /api/v1/genres/{id}/exists`: Перевірка існування жанру (вимагає авторизації).
*   `POST /api/v1/genres`: Створення нового жанру (лише Admin).
*   `PUT /api/v1/genres/{id}`: Оновлення жанру (лише Admin).
*   `DELETE /api/v1/genres/{id}`: Видалення жанру (лише Admin).

### Сеанси (Sessions)
*   `GET /api/v1/sessions/filter`: Отримання списку сеансів з фільтрацією (доступно анонімно).
*   `GET /api/v1/sessions/filter-with-content`: Отримання списку сеансів разом з деталями контенту (доступно анонімно).
*   `GET /api/v1/sessions`: Отримання всіх сеансів (лише Admin).
*   `GET /api/v1/sessions/{id}`: Отримання сеансу за ID (доступно анонімно).
*   `GET /api/v1/sessions/{id}/exists`: Перевірка існування сеансу (вимагає авторизації).
*   `POST /api/v1/sessions`: Створення нового сеансу (лише Admin).
*   `PUT /api/v1/sessions/{id}`: Оновлення сеансу (лише Admin).
*   `DELETE /api/v1/sessions/{id}`: Видалення сеансу (лише Admin).

### Користувачі (Users)
*   `GET /api/v1/users/filter`: Отримання списку користувачів з фільтрацією (лише Admin).
*   `GET /api/v1/users/{id}/exists`: Перевірка існування користувача за ID (лише Admin).
*   `GET /api/v1/users/{id}/info`: Отримання базової інформації про користувача за ID (лише Admin).
*   `GET /api/v1/users/{id}`: Отримання повної DTO користувача за ID (лише Admin).
*   `GET /api/v1/users/my-info`: Отримання інформації про поточного авторизованого користувача.
*   `GET /api/v1/users/roles`: Отримання списку всіх доступних ролей (лише Admin).
*   `POST /api/v1/users/admins/register`: Реєстрація нового адміністратора (лише Admin).
*   `POST /api/v1/users/customer/register`: Реєстрація нового клієнта (доступно анонімно).
*   `PUT /api/v1/users/{id}`: Оновлення даних користувача (лише Admin).
*   `DELETE /api/v1/users/{id}`: Видалення користувача (лише Admin).
*   **Вподобаний контент:**
    *   `POST /api/v1/users/favorites/{contentId}`: Додати контент до вподобань (авторизований користувач).
    *   `DELETE /api/v1/users/favorites/{contentId}`: Видалити контент з вподобань (авторизований користувач).
    *   `GET /api/v1/users/favorites/{contentId}/exists`: Перевірити, чи є контент у вподобаннях (авторизований користувач).

### Автентифікація (Auth)
*   `POST /api/v1/auth`: Вхід користувача, отримання JWT токена.

## 🔑 Автентифікація та Авторизація

*   **ASP.NET Core Identity:** Використовується для управління користувачами, паролями, ролями.
*   **JWT (JSON Web Tokens):**
    *   Генеруються при успішному вході (`AuthenticationController`).
    *   Містять інформацію про користувача (ID, роль) у `claims`.
    *   Налаштування JWT (секрет, емітент, аудиторія, час життя) знаходяться в `appsettings.json` (`JwtOptions`).
    *   Клас `JwtAuthentication.cs` відповідає за генерацію токенів.
*   **Ролі:**
    *   Визначені в `Domain/RoleList.cs` (`Admin`, `Customer`).
    *   Використовуються для захисту ендпоінтів за допомогою атрибута `[Authorize(Roles = "RoleName")]`.
*   **Захист ендпоінтів:**
    *   Більшість ендпоінтів вимагають автентифікації (`[Authorize]`).
    *   Деякі ендпоінти доступні анонімно (`[AllowAnonymous]`).
    *   Адміністративні ендпоінти захищені роллю `Admin`.

## 🚦 Обробка Помилок

*   **Result Pattern:**
    *   Сервіси `Application` шару повертають `Result` або `Result<TValue>`.
    *   Успішний результат містить дані (для `Result<TValue>`) або просто позначає успіх (для `Result`).
    *   Невдалий результат містить об'єкт `Error` з кодом, описом та типом помилки (`ErrorType`).
*   **Спеціалізовані класи помилок:** Для кожної сутності часто є свій клас помилок (напр., `ActorErrors`, `ContentErrors`, `UserErrors`), що успадковується від `EntityErrors<TEntity, TId>`.
*   **Мапування на HTTP статуси:**
    *   Розширення `ResultExtensions.cs` в `Web.API/Core` перетворює `Result` об'єкти на `IActionResult`.
    *   `ErrorType` мапиться на відповідні HTTP статус-коди (напр., `NotFound` -> 404, `BadRequest` -> 400, `Conflict` -> 409, `AccessUnAuthorized` -> 401, `AccessForbidden` -> 403).
*   **Валідація запитів:** Атрибути валідації на DTO запитів (`Requests`) автоматично обробляються ASP.NET Core, повертаючи `400 Bad Request` з `ValidationProblemDetails`.

## ⚙️ Конфігурація

*   **`appsettings.json` та `appsettings.Development.json`:** Містять налаштування для різних середовищ.
*   **Ключові налаштування:**
    *   `ConnectionStrings:DataBaseConnection`: Рядок підключення до бази даних MySQL.
    *   `JWT`: Параметри для JWT токенів (Secret, Issuer, Audience, AccessExpirationInMinutes).
    *   `ApplyMigrations`: Чи застосовувати міграції при старті додатку.
    *   `FakeDataSeed:Enabled`: Чи дозволено сідінг даних через API.
    *   `BookingConfirmation`: Параметри для фонового сервісу підтвердження бронювань.
    *   `PublicDataFolder`, `ContentPosterPath`, `ContentBannerPath`, `ActorPhotoPath`: Шляхи для збереження файлів.
    *   `ContentPosterMaxSizeInKilobytes` (та аналогічні): Максимальний розмір файлів.
    *   `InitialAdmin`: Дані для створення початкового адміністратора.

## 🖼️ Робота з Файлами

*   **Інтерфейс:** `IContentFileStorageService` (`Application/Files/Abstractions`) визначає контракт для збереження та видалення файлів.
*   **Реалізація:** `LocalContentFileStorageService` (`Infrastructure/Files`) зберігає файли на локальному диску сервера у публічній директорії (`PublicDataFolder` з конфігурації).
*   **Функціонал:**
    *   Збереження зображень з Base64 рядка (для постерів, банерів, фото акторів).
    *   Видалення файлів за відносним URL.
    *   Автоматичне визначення розширення файлу за його вмістом (для JPG, PNG, WEBP).
    *   Формування унікальних імен файлів.
*   **URL файлів:** API повертає відносні URL до файлів. Фронтенд повинен додавати базовий URL API для отримання повного шляху. Контролери мають метод `CreateFullImageUrl` для цього.

## 🗄️ База Даних

*   **СУБД:** **MySQL** (використовується `Pomelo.EntityFrameworkCore.MySql`).
*   **ORM:** **Entity Framework Core**.
*   **Підхід:** **Code First**. Схема бази даних генерується та оновлюється на основі моделей C# в `Domain/Entities` та конфігурацій в `CoreDbContext`.
*   **Міграції:** Використовуються для еволюційного управління схемою БД. Зберігаються в `Infrastructure/Migrations`.
*   **Контекст даних:** `CoreDbContext.cs` (`Infrastructure/Core`) містить налаштування DbSet-ів, зв'язків між сутностями та логіку автоматичного заповнення полів `CreatedAt` та `UpdatedAt`.
*   **Іменування:** Використовується `snake_case` для імен таблиць та стовпців в базі даних (налаштовується через `UseSnakeCaseNamingConvention()`).
*   **Індекси та обмеження:** Визначаються за допомогою атрибутів даних та Fluent API в `CoreDbContext`.

## 🌱 Сідінг Даних (Data Seeding)

*   **`ISeedService` / `SeedService.cs` (`Application/Seeds`):** Відповідає за заповнення бази даних початковими/тестовими даними.
*   **Можливості:**
    *   Генерація фейкових жанрів, акторів, кінозалів, контенту, сеансів, користувачів (клієнтів), бронювань.
    *   Використання бібліотеки **Bogus** для генерації реалістичних даних.
    *   Можливість завантаження плейсхолдер-зображень для акторів та контенту.
*   **Запуск:**
    *   Може бути запущений через спеціальний ендпоінт `POST /api/v1/utils/seed` (захищений роллю Admin та налаштуванням `FakeDataSeed:Enabled` в `appsettings.json`).
    *   Можливість створення початкового адміністратора при старті додатку (`AppInitializer.cs`) на основі конфігурації `InitialAdmin` в `appsettings.json`.
*   **Параметри сідінгу:** Кількість сутностей кожного типу, необхідність завантаження зображень тощо, можна передавати як параметри запиту до ендпоінту.

---
<a href="https://github.com/Rivgoo/MovieHub-API">MovieHub-API</a> © 2025 by <a href="https://github.com/Rivgoo">Pavlo Panko</a> is licensed under <a href="https://creativecommons.org/licenses/by-nc/4.0/">CC BY-NC 4.0</a><img src="https://mirrors.creativecommons.org/presskit/icons/cc.svg" style="max-width: 1em;max-height:1em;margin-left: .2em;"><img src="https://mirrors.creativecommons.org/presskit/icons/by.svg" style="max-width: 1em;max-height:1em;margin-left: .2em;"><img src="https://mirrors.creativecommons.org/presskit/icons/nc.svg" style="max-width: 1em;max-height:1em;margin-left: .2em;">