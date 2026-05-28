# ─── Build stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first (layer cache for restore)
COPY Flowtap_API.sln ./
COPY Flowtap_Domain/Flowtap_Domain.csproj Flowtap_Domain/
COPY Flowtap_Application/Flowtap_Application.csproj Flowtap_Application/
COPY Flowtap_Infrastructure/Flowtap_Infrastructure.csproj Flowtap_Infrastructure/
COPY Flowtap_Configuration/Flowtap_Configuration.csproj Flowtap_Configuration/
COPY Flowtap_Middleware/Flowtap_Middleware.csproj Flowtap_Middleware/
COPY Flowtap_Presentation/Flowtap_Presentation.csproj Flowtap_Presentation/

RUN dotnet restore

# Copy all source files
COPY . .

# Publish release build
RUN dotnet publish Flowtap_Presentation/Flowtap_Presentation.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ─── Runtime stage ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create wwwroot/uploads directory for local file storage
RUN mkdir -p wwwroot/uploads

COPY --from=build /app/publish .

EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Flowtap_Presentation.dll"]
