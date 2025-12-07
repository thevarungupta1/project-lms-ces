#!/bin/bash

# Complete Azure Container Instances Deployment Script
# This script handles deployment, CORS configuration, and seed script execution

set -e

# ============================================
# CONFIGURATION - UPDATE THESE VALUES
# ============================================
RESOURCE_GROUP="ces-lms-rg"
LOCATION="eastus"
REGISTRY_NAME="your-registry"
BACKEND_IMAGE="${REGISTRY_NAME}.azurecr.io/ces-lms-backend:latest"
FRONTEND_IMAGE="${REGISTRY_NAME}.azurecr.io/ces-lms-frontend:latest"

# MongoDB Connection (use MongoDB Atlas or Azure Cosmos DB)
MONGODB_URI="${MONGODB_URI:-mongodb+srv://username:password@cluster.mongodb.net/ces-lms?retryWrites=true&w=majority}"

# JWT Secrets (generate secure random strings)
JWT_SECRET="${JWT_SECRET:-$(openssl rand -base64 32)}"
JWT_REFRESH_SECRET="${JWT_REFRESH_SECRET:-$(openssl rand -base64 32)}"

# ============================================
# FUNCTIONS
# ============================================

log() {
  echo "📋 $1"
}

error() {
  echo "❌ $1" >&2
  exit 1
}

# ============================================
# DEPLOYMENT
# ============================================

log "Starting Azure Container Instances deployment..."

# Create resource group
log "Creating resource group: $RESOURCE_GROUP"
az group create --name $RESOURCE_GROUP --location $LOCATION || log "Resource group already exists"

# Get ACR credentials
log "Getting ACR credentials..."
ACR_USERNAME=$(az acr credential show --name $REGISTRY_NAME --query username --output tsv)
ACR_PASSWORD=$(az acr credential show --name $REGISTRY_NAME --query passwords[0].value --output tsv)

# Deploy backend container
log "Deploying backend container..."
az container create \
  --resource-group $RESOURCE_GROUP \
  --name ces-lms-backend \
  --image $BACKEND_IMAGE \
  --cpu 1 \
  --memory 1.5 \
  --registry-login-server ${REGISTRY_NAME}.azurecr.io \
  --registry-username $ACR_USERNAME \
  --registry-password $ACR_PASSWORD \
  --ip-address Public \
  --ports 3000 \
  --dns-name-label ces-lms-backend-$(date +%s) \
  --environment-variables \
    NODE_ENV=production \
    PORT=3000 \
    API_VERSION=v1 \
    MONGODB_URI="$MONGODB_URI" \
    MONGODB_DB_NAME=ces-lms \
    JWT_SECRET="$JWT_SECRET" \
    JWT_EXPIRES_IN=7d \
    JWT_REFRESH_SECRET="$JWT_REFRESH_SECRET" \
    JWT_REFRESH_EXPIRES_IN=30d \
    BCRYPT_ROUNDS=12 \
    CORS_CREDENTIALS=true \
    RATE_LIMIT_WINDOW_MS=900000 \
    RATE_LIMIT_MAX_REQUESTS=100 \
    LOG_LEVEL=info \
  --restart-policy Always || error "Failed to deploy backend"

# Get backend URL
BACKEND_FQDN=$(az container show \
  --resource-group $RESOURCE_GROUP \
  --name ces-lms-backend \
  --query "ipAddress.fqdn" \
  --output tsv)

BACKEND_URL="http://${BACKEND_FQDN}:3000"
log "✅ Backend deployed at: $BACKEND_URL"

# Wait for backend to be ready
log "Waiting for backend to be ready..."
sleep 10

# Update CORS with backend URL (will be updated again after frontend deployment)
log "Updating CORS configuration..."
az container update \
  --resource-group $RESOURCE_GROUP \
  --name ces-lms-backend \
  --set-env-variables CORS_ORIGIN="http://${BACKEND_FQDN}:3000" \
  --restart-policy Always || log "CORS update failed, will update after frontend deployment"

# Deploy frontend container
log "Deploying frontend container..."
az container create \
  --resource-group $RESOURCE_GROUP \
  --name ces-lms-frontend \
  --image $FRONTEND_IMAGE \
  --cpu 0.5 \
  --memory 0.5 \
  --registry-login-server ${REGISTRY_NAME}.azurecr.io \
  --registry-username $ACR_USERNAME \
  --registry-password $ACR_PASSWORD \
  --ip-address Public \
  --ports 80 \
  --dns-name-label ces-lms-frontend-$(date +%s) \
  --restart-policy Always || error "Failed to deploy frontend"

# Get frontend URL
FRONTEND_FQDN=$(az container show \
  --resource-group $RESOURCE_GROUP \
  --name ces-lms-frontend \
  --query "ipAddress.fqdn" \
  --output tsv)

FRONTEND_URL="http://${FRONTEND_FQDN}"
log "✅ Frontend deployed at: $FRONTEND_URL"

# Update CORS with frontend URL
log "Updating CORS configuration with frontend URL..."
az container update \
  --resource-group $RESOURCE_GROUP \
  --name ces-lms-backend \
  --set-env-variables CORS_ORIGIN="$FRONTEND_URL" \
  --restart-policy Always || error "Failed to update CORS"

# Restart backend to apply CORS changes
log "Restarting backend to apply CORS changes..."
az container restart \
  --resource-group $RESOURCE_GROUP \
  --name ces-lms-backend

log "⏳ Waiting for backend to restart..."
sleep 15

# ============================================
# SEED SCRIPT EXECUTION
# ============================================

log "🌱 Running seed script..."

SEED_CONTAINER_NAME="ces-lms-seed-$(date +%s)"

# Create temporary container to run seed script
az container create \
  --resource-group $RESOURCE_GROUP \
  --name $SEED_CONTAINER_NAME \
  --image $BACKEND_IMAGE \
  --cpu 1 \
  --memory 1 \
  --registry-login-server ${REGISTRY_NAME}.azurecr.io \
  --registry-username $ACR_USERNAME \
  --registry-password $ACR_PASSWORD \
  --restart-policy Never \
  --command-line "npm run seed" \
  --environment-variables \
    NODE_ENV=production \
    MONGODB_URI="$MONGODB_URI" \
    MONGODB_DB_NAME=ces-lms || error "Failed to create seed container"

log "⏳ Waiting for seed script to complete..."

# Stream logs and wait for completion
az container logs \
  --resource-group $RESOURCE_GROUP \
  --name $SEED_CONTAINER_NAME \
  --follow

# Get exit code
EXIT_CODE=$(az container show \
  --resource-group $RESOURCE_GROUP \
  --name $SEED_CONTAINER_NAME \
  --query "containers[0].instanceView.currentState.exitCode" \
  --output tsv)

# Clean up seed container
log "🧹 Cleaning up seed container..."
az container delete \
  --resource-group $RESOURCE_GROUP \
  --name $SEED_CONTAINER_NAME \
  --yes

if [ "$EXIT_CODE" == "0" ]; then
  log "✅ Seed script completed successfully!"
else
  error "Seed script failed with exit code: $EXIT_CODE"
fi

# ============================================
# SUMMARY
# ============================================

echo ""
echo "🎉 Deployment Complete!"
echo "======================"
echo "Backend URL:  $BACKEND_URL"
echo "Frontend URL: $FRONTEND_URL"
echo ""
echo "📝 Next Steps:"
echo "1. Update frontend build with correct API URL: $BACKEND_URL"
echo "2. Rebuild and redeploy frontend if needed"
echo "3. Test the application"
echo "4. Configure custom domains if needed"
echo ""
echo "🔐 Security Notes:"
echo "- JWT secrets have been generated"
echo "- Update CORS_ORIGIN if using custom domains"
echo "- Consider using Azure Key Vault for secrets"
echo ""

