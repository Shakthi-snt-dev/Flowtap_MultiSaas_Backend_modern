#!/bin/bash
# ─── Flowtap Main API (all industries) — Run All Migrations ──────────────────
# Run from solution root: bash migrate-main.sh

set -e
ROOT="$(cd "$(dirname "$0")" && pwd)"
cd "$ROOT"

echo "=== Flowtap Main API Migrations (all contexts) ==="

echo "[1/6] Applying ApplicationDbContext (shared core)..."
dotnet ef database update \
  --project  SharedCore/Flowtap_Infrastructure/Flowtap_Infrastructure.csproj \
  --startup-project APIEndpoints/Flowtap_Presentation/Flowtap_Presentation.csproj \
  --context  ApplicationDbContext
echo "    ✓ done"

echo "[2/6] Applying FoodDbContext..."
dotnet ef database update \
  --project  IndustryModules/Flowtap_Food/Flowtap_Food.csproj \
  --startup-project APIEndpoints/Flowtap_Presentation/Flowtap_Presentation.csproj \
  --context  FoodDbContext
echo "    ✓ done"

echo "[3/6] Applying RepairDbContext..."
dotnet ef database update \
  --project  IndustryModules/Flowtap_Repair/Flowtap_Repair.csproj \
  --startup-project APIEndpoints/Flowtap_Presentation/Flowtap_Presentation.csproj \
  --context  RepairDbContext
echo "    ✓ done"

echo "[4/6] Applying HotelDbContext..."
dotnet ef database update \
  --project  IndustryModules/Flowtap_Hotel/Flowtap_Hotel.csproj \
  --startup-project APIEndpoints/Flowtap_Presentation/Flowtap_Presentation.csproj \
  --context  HotelDbContext
echo "    ✓ done"

echo "[5/6] Applying MedicalDbContext..."
dotnet ef database update \
  --project  IndustryModules/Flowtap_Medical/Flowtap_Medical.csproj \
  --startup-project APIEndpoints/Flowtap_Presentation/Flowtap_Presentation.csproj \
  --context  MedicalDbContext
echo "    ✓ done"

echo "[6/6] Applying JewelryDbContext..."
dotnet ef database update \
  --project  IndustryModules/Flowtap_Jewelry/Flowtap_Jewelry.csproj \
  --startup-project APIEndpoints/Flowtap_Presentation/Flowtap_Presentation.csproj \
  --context  JewelryDbContext
echo "    ✓ done"

echo ""
echo "✅ Main API — all database migrations complete."
