#!/bin/bash
# Helper script to run the BapMate WebApi project with a specific profile.
# Defaults to the 'http' profile if not specified.
# Usage:
#   ./run.sh                  (runs 'http' profile)
#   ./run.sh https            (runs 'https' profile)
#   ./run.sh ProductionLocal  (runs 'ProductionLocal' profile)

PROFILE=${1:-http}
echo "🚀 Starting BapMate.WebApi with profile: '$PROFILE'..."
dotnet run --project BapMate.WebApi --profile "$PROFILE"
