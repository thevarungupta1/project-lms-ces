#!/bin/bash

# ============================================
# Fix Docker Build Issues on Azure Ubuntu
# ============================================
# This script fixes common Docker build issues:
# 1. Corrupted/empty Dockerfile
# 2. Network issues pulling base images
# Usage: bash fix-docker-issues.sh

set -e

echo "============================================"
echo "Fixing Docker Build Issues"
echo "============================================"
echo ""

# Fix 1: Recreate Frontend Dockerfile
echo "📝 Fixing frontend Dockerfile..."
cat > react-web-app/Dockerfile << 'EOF'
# Multi-stage build for React frontend
FROM node:18-alpine AS builder

# Set working directory
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci

# Copy source code and config files
COPY . .

# Build arguments for environment variables
ARG VITE_API_BASE_URL
ARG VITE_API_VERSION
ARG VITE_API_TIMEOUT

# Set environment variables for build
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
ENV VITE_API_VERSION=$VITE_API_VERSION
ENV VITE_API_TIMEOUT=$VITE_API_TIMEOUT

# Build the application
RUN npm run build

# Production stage with nginx
FROM nginx:alpine AS production

# Copy built application from builder
COPY --from=builder /app/dist /usr/share/nginx/html

# Add health check script
RUN echo '#!/bin/sh' > /healthcheck.sh && \
    echo 'wget --quiet --tries=1 --spider http://localhost/ || exit 1' >> /healthcheck.sh && \
    chmod +x /healthcheck.sh

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD /healthcheck.sh

# Expose port
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
EOF

echo "✅ Frontend Dockerfile recreated"
echo ""

# Verify Dockerfile
if [ -f react-web-app/Dockerfile ]; then
    FILE_SIZE=$(wc -c < react-web-app/Dockerfile)
    LINE_COUNT=$(wc -l < react-web-app/Dockerfile)
    echo "   File size: ${FILE_SIZE} bytes"
    echo "   Line count: ${LINE_COUNT} lines"
    
    if [ "$FILE_SIZE" -lt 100 ]; then
        echo "   ⚠️  Warning: File seems too small!"
    else
        echo "   ✅ File looks good"
    fi
else
    echo "   ❌ Error: Dockerfile not found!"
    exit 1
fi

echo ""

# Fix 2: Pre-pull base images to avoid network issues
echo "🔄 Pre-pulling Docker base images..."
echo "   This may take a few minutes..."
echo ""

# Pull base images with retry logic
pull_image_with_retry() {
    local image=$1
    local max_attempts=3
    local attempt=1
    
    while [ $attempt -le $max_attempts ]; do
        echo "   Attempt $attempt/$max_attempts: Pulling $image..."
        if sudo docker pull "$image"; then
            echo "   ✅ Successfully pulled $image"
            return 0
        else
            echo "   ⚠️  Failed to pull $image (attempt $attempt/$max_attempts)"
            if [ $attempt -lt $max_attempts ]; then
                echo "   Waiting 5 seconds before retry..."
                sleep 5
            fi
            attempt=$((attempt + 1))
        fi
    done
    
    echo "   ❌ Failed to pull $image after $max_attempts attempts"
    return 1
}

# Pull required base images
echo "Pulling node:18-alpine..."
pull_image_with_retry "node:18-alpine" || echo "   ⚠️  Will retry during build"

echo ""
echo "Pulling nginx:alpine..."
pull_image_with_retry "nginx:alpine" || echo "   ⚠️  Will retry during build"

echo ""

# Fix 3: Verify Docker and Docker Compose
echo "🔍 Verifying Docker setup..."
if command -v docker &> /dev/null; then
    echo "   ✅ Docker is installed"
    sudo docker --version
else
    echo "   ❌ Docker is not installed!"
    exit 1
fi

if command -v docker-compose &> /dev/null || docker compose version &> /dev/null; then
    echo "   ✅ Docker Compose is installed"
    docker compose version 2>/dev/null || docker-compose --version
else
    echo "   ❌ Docker Compose is not installed!"
    exit 1
fi

echo ""

# Fix 4: Check network connectivity
echo "🌐 Checking network connectivity..."
if ping -c 1 -W 2 registry-1.docker.io &> /dev/null || ping -c 1 -W 2 hub.docker.com &> /dev/null; then
    echo "   ✅ Can reach Docker Hub"
else
    echo "   ⚠️  Cannot reach Docker Hub - check your internet connection"
    echo "   You may need to configure a Docker Hub mirror or proxy"
fi

echo ""

# Fix 5: Clean Docker cache (optional)
read -p "Do you want to clean Docker build cache? (y/N): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo "🧹 Cleaning Docker build cache..."
    sudo docker builder prune -f
    echo "   ✅ Build cache cleaned"
fi

echo ""
echo "============================================"
echo "Fix Complete!"
echo "============================================"
echo ""
echo "📋 Summary:"
echo "   ✅ Frontend Dockerfile recreated"
echo "   ✅ Base images pre-pulled (if successful)"
echo "   ✅ Docker setup verified"
echo ""
echo "🚀 Next steps:"
echo "   1. Verify .env file is configured correctly"
echo "   2. Run: sudo docker compose up -d --build"
echo ""
echo "💡 If you still get network errors:"
echo "   - Check internet connection"
echo "   - Try again later (Docker Hub may be temporarily unavailable)"
echo "   - Consider using a Docker Hub mirror"
echo ""

