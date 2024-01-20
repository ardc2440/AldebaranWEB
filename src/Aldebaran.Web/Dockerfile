#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Aldebaran.Web/Aldebaran.Web.csproj", "Aldebaran.Web/"]
COPY ["Aldebaran.Application.Services/Aldebaran.Application.Services.csproj", "Aldebaran.Application.Services/"]
COPY ["Aldebaran.DataAccess.Infraestructure/Aldebaran.DataAccess.Infraestructure.csproj", "Aldebaran.DataAccess.Infraestructure/"]
COPY ["Aldebaran.DataAccess/Aldebaran.DataAccess.csproj", "Aldebaran.DataAccess/"]
COPY ["Aldebaran.Infraestructure.Common/Aldebaran.Infraestructure.Common.csproj", "Aldebaran.Infraestructure.Common/"]
RUN dotnet restore "./Aldebaran.Web/./Aldebaran.Web.csproj"
COPY . .
WORKDIR "/src/Aldebaran.Web"
RUN dotnet build "./Aldebaran.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Aldebaran.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aldebaran.Web.dll"]