#!/bin/bash
# ─── Flowtap Medical API — Run All Migrations ────────────────────────────────
# Run from solution root: bash migrate-medical.sh

set -e
ROOT="$(cd "$(dirname "$0")" && pwd)"
cd "$ROOT"

echo "=== Flowtap Medical API Migrations ==="

echo "[1/2] Applying ApplicationDbContext (shared core tables)..."
dotnet ef database update \
  --project  SharedCore/Flowtap_Infrastructure/Flowtap_Infrastructure.csproj \
  --startup-project APIEndpoints/Flowtap_Medical_API/Flowtap_Medical_API.csproj \
  --context  ApplicationDbContext
echo "    ✓ ApplicationDbContext done"

echo "[2/2] Applying MedicalDbContext (medical-specific tables)..."
dotnet ef database update \
  --project  IndustryModules/Flowtap_Medical/Flowtap_Medical.csproj \
  --startup-project APIEndpoints/Flowtap_Medical_API/Flowtap_Medical_API.csproj \
  --context  MedicalDbContext
echo "    ✓ MedicalDbContext done"

echo ""
echo "✅ Medical database migration complete."
