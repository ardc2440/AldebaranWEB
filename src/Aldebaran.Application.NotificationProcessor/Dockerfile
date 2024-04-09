#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Aldebaran.Application.NotificationProcessor/Aldebaran.Application.NotificationProcessor.csproj", "Aldebaran.Application.NotificationProcessor/"]
COPY ["Aldebaran.Application.Services.Notificator/Aldebaran.Application.Services.Notificator.csproj", "Aldebaran.Application.Services.Notificator/"]
COPY ["Aldebaran.DataAccess.Infraestructure/Aldebaran.DataAccess.Infraestructure.csproj", "Aldebaran.DataAccess.Infraestructure/"]
COPY ["Aldebaran.DataAccess/Aldebaran.DataAccess.csproj", "Aldebaran.DataAccess/"]
COPY ["Aldebaran.Infraestructure.Common/Aldebaran.Infraestructure.Common.csproj", "Aldebaran.Infraestructure.Common/"]
COPY ["Aldebaran.Infraestructure.Core/Aldebaran.Infraestructure.Core.csproj", "Aldebaran.Infraestructure.Core/"]
RUN dotnet restore "./Aldebaran.Application.NotificationProcessor/./Aldebaran.Application.NotificationProcessor.csproj"
COPY . .
WORKDIR "/src/Aldebaran.Application.NotificationProcessor"
RUN dotnet build "./Aldebaran.Application.NotificationProcessor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Aldebaran.Application.NotificationProcessor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aldebaran.Application.NotificationProcessor.dll"]