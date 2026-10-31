# Контрольная работа №9 — электронный кошелёк

Проект разделён на три уровня: `WebApplication1.Web`, `WebApplication1.BLL` и `WebApplication1.DAL`. Веб-уровень использует ASP.NET Core Identity; сущности Identity, кошельки, счета поставщиков и журнал операций хранятся в PostgreSQL через EF Core и репозитории.

## Запуск

1. Запусти локальный PostgreSQL и создай пользователя/базу либо используй пользователя, которому разрешено создавать базу.
2. В PowerShell задай строку подключения (подставь собственный пароль локально, не сохраняй его в Git):

```powershell
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Port=5432;Database=wallet_control_9;Username=postgres;Password=<твой-пароль>"
dotnet run --project .\WebApplication1.Web\WebApplication1.Web.csproj
```

При первом запуске приложение создаёт схему и демо-счета поставщиков. Зарегистрированному пользователю назначается случайный уникальный шестизначный номер и демонстрационный начальный баланс 100 000 сом.

## Демо-счета услуг

- Mobilink — `0555123456`
- FiberNet — `12345678`
- City Utilities — `LC-100200`

## Трекер задач

Состав и критерии задач описаны в [docs/TaskTracker.md](docs/TaskTracker.md). Задачи ведутся в [GitHub Issues](https://github.com/sam1asangzv/ControlWork9_Samat_Asangaziev/issues), а коммиты содержат ссылки на соответствующие номера задач.
