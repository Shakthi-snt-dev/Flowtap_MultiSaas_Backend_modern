#!/bin/bash
# ─── Generate SQL migration scripts for Repair API ───────────────────────────
set -e
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

OUT="scripts/sql/repair"
mkdir -p "$OUT"

echo "=== Generating SQL scripts for Repair API ==="

echo "[1/2] ApplicationDbContext → $OUT/01_application.sql"
dotnet ef migrations script \
  --idempotent \
  --output "$OUT/01_application.sql" \
  --project  SharedCore/Flowtap_Infrastructure/Flowtap_Infrastructure.csproj \
  --startup-project APIEndpoints/Flowtap_Repair_API/Flowtap_Repair_API.csproj \
  --context  ApplicationDbContext
echo "    ✓ done"

echo "[2/2] RepairDbContext → $OUT/02_repair.sql"
dotnet ef migrations script \
  --idempotent \
  --output "$OUT/02_repair.sql" \
  --project  IndustryModules/Flowtap_Repair/Flowtap_Repair.csproj \
  --startup-project APIEndpoints/Flowtap_Repair_API/Flowtap_Repair_API.csproj \
  --context  RepairDbContext
echo "    ✓ done"

echo ""
echo "✅ SQL scripts written to $OUT/"
ls -lh "$OUT/"
