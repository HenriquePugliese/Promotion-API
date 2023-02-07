# Acropolis API

## Patterns Used In This Application

- [Domain Driven Design](https://balta.io/cursos/modelando-dominios-ricos)
- [Folder By Feature Structure](https://github.com/tfsantosbr/dotnet-folder-by-feature-structure)

## Running Docker Infrastructure Dependencies

You will need [Docker Desktop](https://docs.docker.com/desktop/install/windows-install/) to run this commands

```bash
docker-compose up -d
```

## Migrations

You will need [.NET EF Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) to run this commands

```bash
# add migration
dotnet ef migrations add {MIGRATION_NAME} -p src/Acropolis.Infrastructure/ -c AcropolisContext -s src/Acropolis.Api -o Contexts/Migrations

# remove migration
dotnet ef migrations remove -p src/Acropolis.Infrastructure/ -c AcropolisContext -s src/Acropolis.Api

# update database
dotnet ef database update -p src/Acropolis.Infrastructure -c AcropolisContext -s src/Acropolis.Api
```

## Running API Locally

You will need [.NET CLI](https://dotnet.microsoft.com/en-us/download) to run this commands

```bash
dotnet restore
dotnet build
dotnet run --project src/Acropolis.Api
```
