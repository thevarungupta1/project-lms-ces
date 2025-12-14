#!/bin/bash
# Quick fix for frontend Dockerfile on Azure VM
# Run: bash fix-frontend-dockerfile.sh

set -e

echo "Fixing frontend Dockerfile..."

# Ensure directory exists
mkdir -p react-web-app

# Remove old file if it exists
rm -f react-web-app/Dockerfile

# Create new Dockerfile
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

# Verify
echo ""
echo "Verifying Dockerfile..."
FILE_SIZE=$(wc -c < react-web-app/Dockerfile)
LINE_COUNT=$(wc -l < react-web-app/Dockerfile)

echo "File size: ${FILE_SIZE} bytes"
echo "Line count: ${LINE_COUNT} lines"

if [ "$FILE_SIZE" -lt 100 ]; then
    echo "ERROR: File is still too small!"
    echo "Checking permissions..."
    ls -la react-web-app/Dockerfile
    exit 1
else
    echo "SUCCESS: Dockerfile created correctly!"
    echo ""
    echo "First 5 lines:"
    head -5 react-web-app/Dockerfile
fi

