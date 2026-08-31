#!/bin/bash

SERVICES=(
  "Fenicia.Auth|5000|Src/APIs/Fenicia.Auth"
  "Fenicia.Module.Basic|5083|Src/APIs/Fenicia.Module.Basic"
  "Fenicia.Module.Projects|5144|Src/APIs/Fenicia.Module.Projects"
  "Fenicia.Module.Accounting|5010|Src/APIs/Fenicia.Module.Accounting"
  "Fenicia.Module.Contracts|5012|Src/APIs/Fenicia.Module.Contracts"
  "Fenicia.Module.CustomerSupport|5014|Src/APIs/Fenicia.Module.CustomerSupport"
  "Fenicia.Module.Ecommerce|5016|Src/APIs/Fenicia.Module.Ecommerce"
  "Fenicia.Module.HR|5018|Src/APIs/Fenicia.Module.HR"
  "Fenicia.Module.PerformanceEvaluation|5020|Src/APIs/Fenicia.Module.PerformanceEvaluation"
  "Fenicia.Module.POS|5022|Src/APIs/Fenicia.Module.POS"
  "Fenicia.Module.Plus|5024|Src/APIs/Fenicia.Module.Plus"
  "Fenicia.Module.SocialNetwork|5026|Src/APIs/Fenicia.Module.SocialNetwork"
)

PIDS=()

printf "%-35s %-8s %s\n" "SERVICE" "PORT" "COMMAND"
printf "%-35s %-8s %s\n" "-----------------------------------" "--------" "--------------------------------------------------"

for service in "${SERVICES[@]}"; do
  IFS='|' read -r name port dir <<< "$service"
  printf "%-35s %-8s %s\n" "$name" "$port" "dotnet watch run --project $dir"
done

printf "%-35s %-8s %s\n" "Frontend (Vite)" "5173" "npm run dev --prefix Src/Front"
echo ""

for service in "${SERVICES[@]}"; do
  IFS='|' read -r name port dir <<< "$service"
  echo "Starting $name on port $port..."
  dotnet watch run --project "$dir" &
  PIDS+=($!)
done

echo "Starting Frontend (Vite) on port 5173..."
npm run dev --prefix Src/Front &
PIDS+=($!)

cleanup() {
  echo ""
  echo "Stopping all services..."
  for pid in "${PIDS[@]}"; do
    if kill -0 "$pid" 2>/dev/null; then
      kill "$pid" 2>/dev/null || true
    fi
  done
  wait
  echo "All services stopped."
  exit 0
}

trap cleanup SIGINT SIGTERM

wait
