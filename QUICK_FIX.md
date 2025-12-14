# Quick Fix for Docker Build Issues

## Problem 1: Frontend Dockerfile is Empty (2 bytes)

The frontend Dockerfile on your server is corrupted or empty. Fix it by running this command on your Azure Ubuntu VM:

```bash
cd ~/project-lms-ces

# Recreate the Dockerfile
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

# Verify it was created correctly
ls -lh react-web-app/Dockerfile
wc -l react-web-app/Dockerfile
```

## Problem 2: Network Error Pulling Base Images

If you get "failed to authorize: Unavailable: error reading from server: EOF", try these solutions:

### Solution 1: Pre-pull Images (Recommended)

```bash
# Pull base images before building
sudo docker pull node:18-alpine
sudo docker pull nginx:alpine

# Then build
sudo docker compose up -d --build
```

### Solution 2: Retry the Build

Sometimes Docker Hub is temporarily unavailable. Just retry:

```bash
sudo docker compose up -d --build
```

### Solution 3: Use Automated Fix Script

Run the comprehensive fix script:

```bash
cd ~/project-lms-ces
bash fix-docker-issues.sh
```

### Solution 4: Check Network Connectivity

```bash
# Test internet connection
ping -c 3 8.8.8.8

# Test Docker Hub connectivity
ping -c 3 registry-1.docker.io

# Check DNS resolution
nslookup registry-1.docker.io
```

### Solution 5: Configure Docker Hub Mirror (If in China/restricted region)

Edit `/etc/docker/daemon.json`:

```bash
sudo nano /etc/docker/daemon.json
```

Add:
```json
{
  "registry-mirrors": [
    "https://docker.mirrors.ustc.edu.cn",
    "https://hub-mirror.c.163.com"
  ]
}
```

Then restart Docker:
```bash
sudo systemctl restart docker
```

## Complete Fix Workflow

1. **SSH into your Azure VM:**
   ```bash
   ssh ubuntu@YOUR_VM_IP
   ```

2. **Navigate to project:**
   ```bash
   cd ~/project-lms-ces
   ```

3. **Run the fix script:**
   ```bash
   bash fix-docker-issues.sh
   ```

4. **Or manually fix:**
   ```bash
   # Fix Dockerfile (use command from Problem 1 above)
   # Pre-pull images
   sudo docker pull node:18-alpine
   sudo docker pull nginx:alpine
   ```

5. **Build and deploy:**
   ```bash
   sudo docker compose up -d --build
   ```

6. **Check status:**
   ```bash
   sudo docker compose ps
   sudo docker compose logs -f
   ```

## Verification Commands

After fixing, verify everything:

```bash
# Check Dockerfile exists and has content
ls -lh react-web-app/Dockerfile
wc -l react-web-app/Dockerfile

# Check images are pulled
sudo docker images | grep -E "node|nginx"

# Test Docker Compose config
sudo docker compose config

# Build without starting
sudo docker compose build

# Start services
sudo docker compose up -d
```

## Still Having Issues?

1. **Check Docker logs:**
   ```bash
   sudo journalctl -u docker.service -n 50
   ```

2. **Check disk space:**
   ```bash
   df -h
   sudo docker system df
   ```

3. **Clean up Docker:**
   ```bash
   sudo docker system prune -a
   ```

4. **Restart Docker service:**
   ```bash
   sudo systemctl restart docker
   ```

