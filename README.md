# Pantry API

## Getting started

### Build

Build: `dotnet build`

### Test

Run Tests: `dotnet test`  
Generate Code Coverage Report: `. /scripts/CalculateTestCoverage.ps1`

### Run

Run standalone: `dotnet run --project src/Pantry.Service/Pantry.Service.csproj`  
Run with docker-compose: `docker-compose --file ./src/Pantry.DockerCompose/docker-compose.yml up`

Be aware that the application might requires runtime dependencies which are only started with docker-compose.

| Http Endpoint | Description |
| ------------- | ----------- |
| /api/doc/ui   | Web API Doc |
| /health/ready | HealthCheck |
| /health/live  | HealthCheck |

## Dependencies

| Kind        | Description |
| ----------- | ----------- |
| Persistence | PostgreSQL  |

## How to

_Further information which is necessary for a developer perform certain tasks. E.g. Add schema migrations._

### EF Core Schema Migrations

- The entity classes are located in `Pantry.Core.Persistence.Entities`.
- The entity type configuration classes are located in `Pantry.Core.Persistence.Mappings`.
- The migrations classes are located in `Pantry.Core.Persistence.Migrations`.

To add a new EF Core Migration, execute the following command from Solution-Folder:

```powershell
dotnet ef migrations add <MigrationName> --project .\src\Pantry.Core\Pantry.Core.csproj -o .\Persistence\Migrations --startup-project .\src\Pantry.Service\Pantry.Service.csproj
```

To remove the last migration, execute the following command from Solution-Folder:

```powershell
dotnet ef migrations remove --force --project .\src\Pantry.Core\Pantry.Core.csproj --startup-project .\src\Pantry.Service\Pantry.Service.csproj
```

To generate the database update scripts for all migrations, execute the following command from Solution-Folder:

```powershell
dotnet ef migrations script --project .\src\Pantry.Core\Pantry.Core.csproj --startup-project .\src\Pantry.Service\Pantry.Service.csproj -i
```
