FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
# Copies .csproj into app
COPY ./Covid19Watcher.API.Public/Covid19Watcher.API.Public.csproj ./Covid19Watcher.API.Public
COPY ./Covid19Watcher.Application/Covid19Watcher.Application.csproj ./Covid19Watcher.Application
# Restore nuget packages
RUN dotnet restore ./Covid19Watcher.Application/
RUN dotnet restore ./Covid19Watcher.API.Public/
COPY ./Covid19Watcher.API.Public/ ./Covid19Watcher.API.Public/
COPY ./Covid19Watcher.Application/ ./Covid19Watcher.Application/
RUN dotnet publish -c Release -o out ./Covid19Watcher.API.Public/

COPY --from=build-env /app/Covid19Watcher.API.Public/out .
WORKDIR /app/Covid19Watcher.API.Public/out
ENTRYPOINT ["dotnet", "Covid19Watcher.API.Public.dll"]