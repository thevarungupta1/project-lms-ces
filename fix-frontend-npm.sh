#!/bin/bash
# Fix frontend npm install issues on Azure VM
# Run: bash fix-frontend-npm.sh

set -e

echo "============================================"
echo "Fixing Frontend NPM Issues"
echo "============================================"
echo ""

cd ~/project-lms-ces || { echo "Error: Could not find project directory"; exit 1; }

# Check if package files exist
echo "Checking package files..."
if [ ! -f "react-web-app/package.json" ]; then
    echo "❌ package.json not found!"
    exit 1
fi
echo "✅ package.json exists"

if [ -f "react-web-app/package-lock.json" ]; then
    echo "✅ package-lock.json exists"
    FILE_SIZE=$(wc -c < react-web-app/package-lock.json)
    echo "   File size: ${FILE_SIZE} bytes"
else
    echo "⚠️  package-lock.json not found (will be generated during install)"
fi

echo ""

# Check npm version
echo "Checking npm version..."
if command -v npm &> /dev/null; then
    npm --version
else
    echo "❌ npm not found!"
    exit 1
fi

echo ""

# Verify Dockerfile
echo "Checking Dockerfile..."
if [ -f "react-web-app/Dockerfile" ]; then
    echo "✅ Dockerfile exists"
    # Check if it uses npm ci or npm install
    if grep -q "npm ci" react-web-app/Dockerfile; then
        echo "⚠️  Dockerfile uses 'npm ci' - this might cause issues"
        echo "   Consider updating to use 'npm install' instead"
    else
        echo "✅ Dockerfile uses 'npm install'"
    fi
else
    echo "❌ Dockerfile not found!"
    exit 1
fi

echo ""
echo "============================================"
echo "Fix Complete!"
echo "============================================"
echo ""
echo "💡 If npm ci still fails, the Dockerfile has been updated"
echo "   to use 'npm install --legacy-peer-deps' instead."
echo ""
echo "🚀 Try building again:"
echo "   sudo docker compose up -d --build"
echo ""

