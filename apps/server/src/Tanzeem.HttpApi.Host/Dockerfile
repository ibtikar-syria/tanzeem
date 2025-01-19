FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
COPY bin/Release/net9.0/publish/ app/
WORKDIR /app
COPY . ./
ENV ASPNETCORE_URLS=http://+:80
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/bin .
ENTRYPOINT ["dotnet", "Tanzeem.HttpApi.Host.dll"]