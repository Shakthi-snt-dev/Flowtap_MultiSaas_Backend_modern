#!/bin/bash
# ─── Flowtap Jewelry API — Run All Migrations ────────────────────────────────
# Run from solution root: bash migrate-jewelry.sh

set -e
ROOT="$(cd "$(dirname "$0")" && pwd)"
cd "$ROOT"

echo "=== Flowtap Jewelry API Migrations ==="

echo "[1/2] Applying ApplicationDbContext (shared core tables)..."
dotnet ef database update \
  --project  SharedCore/Flowtap_Infrastructure/Flowtap_Infrastructure.csproj \
  --startup-project APIEndpoints/Flowtap_Jewelry_API/Flowtap_Jewelry_API.csproj \
  --context  ApplicationDbContext
echo "    ✓ ApplicationDbContext done"

echo "[2/2] Applying JewelryDbContext (jewelry-specific tables)..."
dotnet ef database update \
  --project  IndustryModules/Flowtap_Jewelry/Flowtap_Jewelry.csproj \
  --startup-project APIEndpoints/Flowtap_Jewelry_API/Flowtap_Jewelry_API.csproj \
  --context  JewelryDbContext
echo "    ✓ JewelryDbContext done"

echo ""
echo "✅ Jewelry database migration complete."
