ARG IMAGE_TAG=6.0

FROM mcr.microsoft.com/dotnet/sdk:$IMAGE_TAG AS build
WORKDIR /app

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:$IMAGE_TAG AS runtime
WORKDIR /app
COPY --from=build /app .