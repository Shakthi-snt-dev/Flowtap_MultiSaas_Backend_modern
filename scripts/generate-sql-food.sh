#!/bin/bash
# ─── Generate SQL migration scripts for Food API ──────────────────────────────
# Output: scripts/sql/food/
# Run from solution root: bash scripts/generate-sql-food.sh

set -e
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

OUT="scripts/sql/food"
mkdir -p "$OUT"

echo "=== Generating SQL scripts for Food API ==="

echo "[1/2] ApplicationDbContext → $OUT/01_application.sql"
dotnet ef migrations script \
  --idempotent \
  --output "$OUT/01_application.sql" \
  --project  SharedCore/Flowtap_Infrastructure/Flowtap_Infrastructure.csproj \
  --startup-project APIEndpoints/Flowtap_Food_API/Flowtap_Food_API.csproj \
  --context  ApplicationDbContext
echo "    ✓ done"

echo "[2/2] FoodDbContext → $OUT/02_food.sql"
dotnet ef migrations script \
  --idempotent \
  --output "$OUT/02_food.sql" \
  --project  IndustryModules/Flowtap_Food/Flowtap_Food.csproj \
  --startup-project APIEndpoints/Flowtap_Food_API/Flowtap_Food_API.csproj \
  --context  FoodDbContext
echo "    ✓ done"

echo ""
echo "✅ SQL scripts written to $OUT/"
ls -lh "$OUT/"
