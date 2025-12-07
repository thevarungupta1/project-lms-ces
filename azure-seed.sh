#!/bin/bash

# Azure Seed Script Runner
# This script runs the seed script in a temporary Azure Container Instance

set -e

# Configuration
RESOURCE_GROUP="ces-lms-rg"
SEED_CONTAINER_NAME="ces-lms-seed-$(date +%s)"
BACKEND_IMAGE="your-registry.azurecr.io/ces-lms-backend:latest"
MONGODB_URI="${MONGODB_URI:-mongodb://your-mongodb-connection-string}"

echo "🌱 Running seed script in Azure Container Instance..."

# Create temporary container to run seed script
az container create \
  --resource-group $RESOURCE_GROUP \
  --name $SEED_CONTAINER_NAME \
  --image $BACKEND_IMAGE \
  --cpu 1 \
  --memory 1 \
  --registry-login-server $(echo $BACKEND_IMAGE | cut -d'/' -f1) \
  --restart-policy Never \
  --command-line "npm run seed" \
  --environment-variables \
    NODE_ENV=production \
    MONGODB_URI="$MONGODB_URI" \
    MONGODB_DB_NAME=ces-lms

echo "⏳ Waiting for seed script to complete..."

# Stream logs
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

# Clean up
echo "🧹 Cleaning up seed container..."
az container delete \
  --resource-group $RESOURCE_GROUP \
  --name $SEED_CONTAINER_NAME \
  --yes

if [ "$EXIT_CODE" == "0" ]; then
  echo "✅ Seed script completed successfully!"
else
  echo "❌ Seed script failed with exit code: $EXIT_CODE"
  exit 1
fi

