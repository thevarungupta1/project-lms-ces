#!/bin/bash

# Azure Container Instances Deployment Script
# This script deploys both frontend and backend containers to Azure

set -e

# Configuration - Update these values
RESOURCE_GROUP="ces-lms-rg"
LOCATION="eastus"
BACKEND_IMAGE="your-registry.azurecr.io/ces-lms-backend:latest"
FRONTEND_IMAGE="your-registry.azurecr.io/ces-lms-frontend:latest"
BACKEND_CONTAINER_NAME="ces-lms-backend"
FRONTEND_CONTAINER_NAME="ces-lms-frontend"

# MongoDB Atlas connection string (or Azure Cosmos DB)
MONGODB_URI="${MONGODB_URI:-mongodb://your-mongodb-connection-string}"
FRONTEND_URL="${FRONTEND_URL:-https://your-frontend-url.azurestaticapps.net}"

echo "🚀 Starting Azure Container Instances deployment..."

# Create resource group if it doesn't exist
echo "📦 Creating resource group..."
az group create --name $RESOURCE_GROUP --location $LOCATION || true

# Deploy backend container
echo "🔧 Deploying backend container..."
az container create \
  --resource-group $RESOURCE_GROUP \
  --name $BACKEND_CONTAINER_NAME \
  --image $BACKEND_IMAGE \
  --cpu 1 \
  --memory 1.5 \
  --registry-login-server $(echo $BACKEND_IMAGE | cut -d'/' -f1) \
  --ip-address Public \
  --ports 3000 \
  --environment-variables \
    NODE_ENV=production \
    PORT=3000 \
    API_VERSION=v1 \
    MONGODB_URI="$MONGODB_URI" \
    MONGODB_DB_NAME=ces-lms \
    JWT_SECRET="${JWT_SECRET:-change-this-in-production}" \
    JWT_EXPIRES_IN=7d \
    JWT_REFRESH_SECRET="${JWT_REFRESH_SECRET:-change-this-in-production}" \
    JWT_REFRESH_EXPIRES_IN=30d \
    BCRYPT_ROUNDS=12 \
    CORS_ORIGIN="$FRONTEND_URL" \
    CORS_CREDENTIALS=true \
    RATE_LIMIT_WINDOW_MS=900000 \
    RATE_LIMIT_MAX_REQUESTS=100 \
    LOG_LEVEL=info

# Get backend IP address
BACKEND_IP=$(az container show \
  --resource-group $RESOURCE_GROUP \
  --name $BACKEND_CONTAINER_NAME \
  --query ipAddress.ip \
  --output tsv)

echo "✅ Backend deployed at: http://$BACKEND_IP:3000"

# Deploy frontend container
echo "🎨 Deploying frontend container..."
az container create \
  --resource-group $RESOURCE_GROUP \
  --name $FRONTEND_CONTAINER_NAME \
  --image $FRONTEND_IMAGE \
  --cpu 0.5 \
  --memory 0.5 \
  --registry-login-server $(echo $FRONTEND_IMAGE | cut -d'/' -f1) \
  --ip-address Public \
  --ports 80 \
  --environment-variables \
    VITE_API_BASE_URL="http://$BACKEND_IP:3000"

# Get frontend IP address
FRONTEND_IP=$(az container show \
  --resource-group $RESOURCE_GROUP \
  --name $FRONTEND_CONTAINER_NAME \
  --query ipAddress.ip \
  --output tsv)

echo "✅ Frontend deployed at: http://$FRONTEND_IP"

echo ""
echo "🎉 Deployment complete!"
echo "Backend: http://$BACKEND_IP:3000"
echo "Frontend: http://$FRONTEND_IP"
echo ""
echo "⚠️  Don't forget to:"
echo "1. Update CORS_ORIGIN with the frontend URL"
echo "2. Run seed script to populate database"
echo "3. Configure custom domains if needed"

