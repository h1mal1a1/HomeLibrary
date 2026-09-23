Для запуска требуется Docker Desktop и .NET 8 SDK.
1.docker compose up -d
2.dotnet restore
3.dotnet run

База данных, таблица и хранимые процедуры создаются автоматически.

Строка подключения к БД находится в appsettings.json:
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=HomeLibrary;User Id=sa;Password=HomeLibrary_123!;TrustServerCertificate=True;"
  }
}
Параметры SQL Server находятся в docker-compose.yml.
Если меняются порт, логин или пароль SQL Server, соответствующие значения необходимо изменить и в appsettings.json.