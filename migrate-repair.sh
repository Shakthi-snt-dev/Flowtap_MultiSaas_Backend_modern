#!/bin/bash
# ─── Flowtap Repair API — Run All Migrations ─────────────────────────────────
# Run from solution root: bash migrate-repair.sh

set -e
ROOT="$(cd "$(dirname "$0")" && pwd)"
cd "$ROOT"

echo "=== Flowtap Repair API Migrations ==="

echo "[1/2] Applying ApplicationDbContext (shared core tables)..."
dotnet ef database update \
  --project  SharedCore/Flowtap_Infrastructure/Flowtap_Infrastructure.csproj \
  --startup-project APIEndpoints/Flowtap_Repair_API/Flowtap_Repair_API.csproj \
  --context  ApplicationDbContext
echo "    ✓ ApplicationDbContext done"

echo "[2/2] Applying RepairDbContext (repair-specific tables)..."
dotnet ef database update \
  --project  IndustryModules/Flowtap_Repair/Flowtap_Repair.csproj \
  --startup-project APIEndpoints/Flowtap_Repair_API/Flowtap_Repair_API.csproj \
  --context  RepairDbContext
echo "    ✓ RepairDbContext done"

echo ""
echo "✅ Repair database migration complete."
