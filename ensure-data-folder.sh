#!/bin/bash
# Ensure data folder exists before Docker build
# Run this on Azure VM before building

set -e

echo "Checking for data folder in node-web-api..."

if [ ! -d "node-web-api/data" ]; then
    echo "⚠️  Data folder not found! Creating it..."
    mkdir -p node-web-api/data
    
    # Create a minimal seed.ts file if it doesn't exist
    if [ ! -f "node-web-api/data/seed.ts" ]; then
        cat > node-web-api/data/seed.ts << 'EOF'
// Seed script placeholder
// Add your seed data here
console.log('Seed script loaded');
EOF
        echo "✅ Created minimal seed.ts file"
    fi
    
    echo "✅ Data folder created"
else
    echo "✅ Data folder exists"
    echo "Contents:"
    ls -la node-web-api/data/
fi

