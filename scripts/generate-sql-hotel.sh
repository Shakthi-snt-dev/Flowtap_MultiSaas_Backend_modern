#!/bin/bash
# ─── Generate SQL migration scripts for Hotel API ────────────────────────────
set -e
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

OUT="scripts/sql/hotel"
mkdir -p "$OUT"

echo "=== Generating SQL scripts for Hotel API ==="

echo "[1/2] ApplicationDbContext → $OUT/01_application.sql"
dotnet ef migrations script \
  --idempotent \
  --output "$OUT/01_application.sql" \
  --project  SharedCore/Flowtap_Infrastructure/Flowtap_Infrastructure.csproj \
  --startup-project APIEndpoints/Flowtap_Hotel_API/Flowtap_Hotel_API.csproj \
  --context  ApplicationDbContext
echo "    ✓ done"

echo "[2/2] HotelDbContext → $OUT/02_hotel.sql"
dotnet ef migrations script \
  --idempotent \
  --output "$OUT/02_hotel.sql" \
  --project  IndustryModules/Flowtap_Hotel/Flowtap_Hotel.csproj \
  --startup-project APIEndpoints/Flowtap_Hotel_API/Flowtap_Hotel_API.csproj \
  --context  HotelDbContext
echo "    ✓ done"

echo ""
echo "✅ SQL scripts written to $OUT/"
ls -lh "$OUT/"
