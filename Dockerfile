FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
# Lets install some usefull tools
RUN apt-get update
RUN apt-get install -y vim
RUN apt-get install -y net-tools
# Create directories
RUN mkdir Covid19Watcher.API.Public
RUN mkdir Covid19Watcher.Application
# Copies .csproj into /app/Covid19Watcher.API.{Public | Application}
COPY ./Covid19Watcher.API.Public/Covid19Watcher.API.Public.csproj ./Covid19Watcher.API.Public
COPY ./Covid19Watcher.Application/Covid19Watcher.Application.csproj ./Covid19Watcher.Application
# Restore nuget packages
RUN dotnet restore ./Covid19Watcher.Application/Covid19Watcher.Application.csproj
RUN dotnet restore ./Covid19Watcher.API.Public/Covid19Watcher.API.Public.csproj
# Copies everything else
COPY ./Covid19Watcher.API.Public/ ./Covid19Watcher.API.Public/
COPY ./Covid19Watcher.Application/ ./Covid19Watcher.Application/
# Publishes Web API
RUN dotnet publish -c Release -o out ./Covid19Watcher.API.Public/Covid19Watcher.API.Public.csproj
EXPOSE 5000
WORKDIR /app/out
# Starts up Web API
ENTRYPOINT ["dotnet", "Covid19Watcher.API.Public.dll"]