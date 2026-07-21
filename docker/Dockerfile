# syntax=docker/dockerfile:1

# ============================================================
# Etapa 1: compilación
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Copiamos primero los archivos de proyecto para aprovechar
# la caché de restauración de paquetes de Docker.
COPY ["src/Consultoria.Domain/Consultoria.Domain.csproj", "src/Consultoria.Domain/"]
COPY ["src/Consultoria.Application/Consultoria.Application.csproj", "src/Consultoria.Application/"]
COPY ["src/Consultoria.Infrastructure/Consultoria.Infrastructure.csproj", "src/Consultoria.Infrastructure/"]
COPY ["src/Consultoria.Api/Consultoria.Api.csproj", "src/Consultoria.Api/"]

RUN dotnet restore "src/Consultoria.Api/Consultoria.Api.csproj"

# Copiamos el resto del código fuente.
COPY . .

WORKDIR "/src/src/Consultoria.Api"

RUN dotnet publish "Consultoria.Api.csproj" \
    --configuration "$BUILD_CONFIGURATION" \
    --output /app/publish \
    /p:UseAppHost=false

# ============================================================
# Etapa 2: ejecución
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

COPY --from=build /app/publish .

# Usuario sin privilegios incluido en las imágenes modernas de .NET.
USER $APP_UID

ENTRYPOINT ["dotnet", "Consultoria.Api.dll"]