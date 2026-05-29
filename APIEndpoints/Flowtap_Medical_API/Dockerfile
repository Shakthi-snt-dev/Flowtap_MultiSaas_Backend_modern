# ─── Flowtap Medical API — Independent Docker Build ──────────────────────────
# Context: run from the SOLUTION ROOT (Flowtap_MultiSaas_Backend_modern/)
#
# Build:   docker build -f APIEndpoints/Flowtap_Medical_API/Dockerfile -t flowtap-medical-api:latest .
# Run:     docker run -p 5004:8080 --env-file APIEndpoints/Flowtap_Medical_API/.env flowtap-medical-api:latest

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SharedCore/Flowtap_Domain/Flowtap_Domain.csproj                 SharedCore/Flowtap_Domain/
COPY SharedCore/Flowtap_Application/Flowtap_Application.csproj       SharedCore/Flowtap_Application/
COPY SharedCore/Flowtap_Infrastructure/Flowtap_Infrastructure.csproj SharedCore/Flowtap_Infrastructure/
COPY SharedCore/Flowtap_Configuration/Flowtap_Configuration.csproj   SharedCore/Flowtap_Configuration/
COPY SharedCore/Flowtap_Middleware/Flowtap_Middleware.csproj         SharedCore/Flowtap_Middleware/
COPY SharedCore/Flowtap_Presentation.Core/Flowtap_Presentation.Core.csproj SharedCore/Flowtap_Presentation.Core/
COPY IndustryModules/Flowtap_Medical/Flowtap_Medical.csproj         IndustryModules/Flowtap_Medical/
COPY APIEndpoints/Flowtap_Medical_API/Flowtap_Medical_API.csproj    APIEndpoints/Flowtap_Medical_API/

RUN dotnet restore APIEndpoints/Flowtap_Medical_API/Flowtap_Medical_API.csproj

COPY . .
RUN dotnet publish APIEndpoints/Flowtap_Medical_API/Flowtap_Medical_API.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Flowtap_Medical_API.dll"]
