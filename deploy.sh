#!/bin/bash
# bapmate_v2_web VPS deployment script
set -e

echo "=== Starting bapmate_v2_web deployment on VPS ==="

cleanup_git_askpass() {
    if [ -n "${GIT_ASKPASS_FILE:-}" ] && [ -f "$GIT_ASKPASS_FILE" ]; then
        rm -f "$GIT_ASKPASS_FILE"
    fi
}
trap cleanup_git_askpass EXIT

if [ -n "${BAPMATE_GITHUB_TOKEN:-}" ]; then
    GIT_ASKPASS_FILE="$(mktemp)"
    chmod 700 "$GIT_ASKPASS_FILE"
    cat > "$GIT_ASKPASS_FILE" <<'EOF'
#!/bin/sh
case "$1" in
  *Username*) printf '%s\n' "x-access-token" ;;
  *Password*) printf '%s\n' "$BAPMATE_GITHUB_TOKEN" ;;
  *) printf '\n' ;;
esac
EOF
    chmod +x "$GIT_ASKPASS_FILE"
    export GIT_ASKPASS="$GIT_ASKPASS_FILE"
    export GIT_TERMINAL_PROMPT=0
fi

# 1. Pull latest code from GitHub
git pull --ff-only origin main

# 2. Build and Deploy
if [ -f "docker-compose.yml" ]; then
    echo "Deploying via Docker Compose..."
    docker compose down
    docker compose up -d --build
elif [ -f "Dockerfile" ]; then
    echo "Deploying via Dockerfile (docker run)..."
    docker build -t bapmate_v2_web .
    docker stop bapmate_v2_web || true
    docker rm bapmate_v2_web || true
    # Run container on port 8083 mapping to app PORT
    docker run -d --name bapmate_v2_web -p 8083:8083 -e PORT=8083 --restart always bapmate_v2_web
else
    echo "Deploying via dotnet publish and local service..."
    dotnet publish BapMate.WebApi/BapMate.WebApi.csproj -c Release -o ./publish
    
    if command -v pm2 &> /dev/null; then
        echo "Restarting via PM2..."
        pm2 restart bapmate_v2_web || pm2 start ./publish/BapMate.WebApi.dll --name bapmate_v2_web -- --urls http://0.0.0.0:8083
    else
        echo "Restarting via systemd service..."
        sudo systemctl restart bapmate_v2_web || echo "Please ensure systemd service 'bapmate_v2_web' is configured"
    fi
fi

echo "=== Deployment finished successfully ==="
