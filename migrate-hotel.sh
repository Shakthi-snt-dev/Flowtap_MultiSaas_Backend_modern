#!/bin/bash
# ─── Flowtap Hotel API — Run All Migrations ──────────────────────────────────
# Run from solution root: bash migrate-hotel.sh

set -e
ROOT="$(cd "$(dirname "$0")" && pwd)"
cd "$ROOT"

echo "=== Flowtap Hotel API Migrations ==="

echo "[1/2] Applying ApplicationDbContext (shared core tables)..."
dotnet ef database update \
  --project  SharedCore/Flowtap_Infrastructure/Flowtap_Infrastructure.csproj \
  --startup-project APIEndpoints/Flowtap_Hotel_API/Flowtap_Hotel_API.csproj \
  --context  ApplicationDbContext
echo "    ✓ ApplicationDbContext done"

echo "[2/2] Applying HotelDbContext (hotel-specific tables)..."
dotnet ef database update \
  --project  IndustryModules/Flowtap_Hotel/Flowtap_Hotel.csproj \
  --startup-project APIEndpoints/Flowtap_Hotel_API/Flowtap_Hotel_API.csproj \
  --context  HotelDbContext
echo "    ✓ HotelDbContext done"

echo ""
echo "✅ Hotel database migration complete."
