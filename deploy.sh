#!/bin/bash
# bapmate_v2_web VPS deployment script
echo "=== Starting bapmate_v2_web deployment on VPS ==="

# 1. Pull latest code from GitHub
git pull origin main

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
    # Run container on port 8081 mapping to app PORT
    docker run -d --name bapmate_v2_web -p 8081:8081 -e PORT=8081 --restart always bapmate_v2_web
else
    echo "Deploying via dotnet publish and local service..."
    dotnet publish BapMate.WebApi/BapMate.WebApi.csproj -c Release -o ./publish
    
    if command -v pm2 &> /dev/null; then
        echo "Restarting via PM2..."
        pm2 restart bapmate_v2_web || pm2 start ./publish/BapMate.WebApi.dll --name bapmate_v2_web -- --urls http://0.0.0.0:8081
    else
        echo "Restarting via systemd service..."
        sudo systemctl restart bapmate_v2_web || echo "Please ensure systemd service 'bapmate_v2_web' is configured"
    fi
fi

echo "=== Deployment finished successfully ==="
