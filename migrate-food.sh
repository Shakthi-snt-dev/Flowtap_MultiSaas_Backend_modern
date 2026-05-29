#!/bin/bash
# ─── Flowtap Food API — Run All Migrations ────────────────────────────────────
# Run from solution root: bash migrate-food.sh
# Requires: dotnet ef tool  →  dotnet tool install -g dotnet-ef

set -e
ROOT="$(cd "$(dirname "$0")" && pwd)"
cd "$ROOT"

echo "=== Flowtap Food API Migrations ==="

echo "[1/2] Applying ApplicationDbContext (shared core tables)..."
dotnet ef database update \
  --project  SharedCore/Flowtap_Infrastructure/Flowtap_Infrastructure.csproj \
  --startup-project APIEndpoints/Flowtap_Food_API/Flowtap_Food_API.csproj \
  --context  ApplicationDbContext
echo "    ✓ ApplicationDbContext done"

echo "[2/2] Applying FoodDbContext (food-specific tables)..."
dotnet ef database update \
  --project  IndustryModules/Flowtap_Food/Flowtap_Food.csproj \
  --startup-project APIEndpoints/Flowtap_Food_API/Flowtap_Food_API.csproj \
  --context  FoodDbContext
echo "    ✓ FoodDbContext done"

echo ""
echo "✅ Food database migration complete."
