FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

COPY . .

RUN dotnet restore

RUN dotnet build --no-restore -c Release

RUN dotnet test --no-restore -c Release