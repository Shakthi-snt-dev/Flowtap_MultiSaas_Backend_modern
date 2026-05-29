#!/bin/bash
# ─── Generate SQL migration scripts for Medical API ──────────────────────────
set -e
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

OUT="scripts/sql/medical"
mkdir -p "$OUT"

echo "=== Generating SQL scripts for Medical API ==="

echo "[1/2] ApplicationDbContext → $OUT/01_application.sql"
dotnet ef migrations script \
  --idempotent \
  --output "$OUT/01_application.sql" \
  --project  SharedCore/Flowtap_Infrastructure/Flowtap_Infrastructure.csproj \
  --startup-project APIEndpoints/Flowtap_Medical_API/Flowtap_Medical_API.csproj \
  --context  ApplicationDbContext
echo "    ✓ done"

echo "[2/2] MedicalDbContext → $OUT/02_medical.sql"
dotnet ef migrations script \
  --idempotent \
  --output "$OUT/02_medical.sql" \
  --project  IndustryModules/Flowtap_Medical/Flowtap_Medical.csproj \
  --startup-project APIEndpoints/Flowtap_Medical_API/Flowtap_Medical_API.csproj \
  --context  MedicalDbContext
echo "    ✓ done"

echo ""
echo "✅ SQL scripts written to $OUT/"
ls -lh "$OUT/"
