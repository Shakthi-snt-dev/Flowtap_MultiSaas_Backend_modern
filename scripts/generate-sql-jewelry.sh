#!/bin/bash
# ─── Generate SQL migration scripts for Jewelry API ──────────────────────────
set -e
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

OUT="scripts/sql/jewelry"
mkdir -p "$OUT"

echo "=== Generating SQL scripts for Jewelry API ==="

echo "[1/2] ApplicationDbContext → $OUT/01_application.sql"
dotnet ef migrations script \
  --idempotent \
  --output "$OUT/01_application.sql" \
  --project  SharedCore/Flowtap_Infrastructure/Flowtap_Infrastructure.csproj \
  --startup-project APIEndpoints/Flowtap_Jewelry_API/Flowtap_Jewelry_API.csproj \
  --context  ApplicationDbContext
echo "    ✓ done"

echo "[2/2] JewelryDbContext → $OUT/02_jewelry.sql"
dotnet ef migrations script \
  --idempotent \
  --output "$OUT/02_jewelry.sql" \
  --project  IndustryModules/Flowtap_Jewelry/Flowtap_Jewelry.csproj \
  --startup-project APIEndpoints/Flowtap_Jewelry_API/Flowtap_Jewelry_API.csproj \
  --context  JewelryDbContext
echo "    ✓ done"

echo ""
echo "✅ SQL scripts written to $OUT/"
ls -lh "$OUT/"
