#!/bin/bash
# ─── Generate SQL migration scripts for ALL industries ───────────────────────
# Run from solution root: bash scripts/generate-sql-all.sh
# Output: scripts/sql/{industry}/01_application.sql + 02_{industry}.sql

set -e
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

SCRIPTS_DIR="$(dirname "$0")"

echo "======================================================"
echo "  Flowtap — Generate SQL Scripts for All Industries"
echo "======================================================"
echo ""

bash "$SCRIPTS_DIR/generate-sql-food.sh"
echo ""
bash "$SCRIPTS_DIR/generate-sql-repair.sh"
echo ""
bash "$SCRIPTS_DIR/generate-sql-hotel.sh"
echo ""
bash "$SCRIPTS_DIR/generate-sql-medical.sh"
echo ""
bash "$SCRIPTS_DIR/generate-sql-jewelry.sh"

echo ""
echo "======================================================"
echo "✅ All SQL scripts generated:"
echo ""
find scripts/sql -name "*.sql" | sort | while read f; do
  size=$(wc -l < "$f")
  echo "  $f  ($size lines)"
done
echo "======================================================"
