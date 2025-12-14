#!/bin/bash
# Fix missing data folder issue on Azure VM
# Run: bash fix-data-folder.sh

set -e

echo "============================================"
echo "Fixing Data Folder Issue"
echo "============================================"
echo ""

cd ~/project-lms-ces || { echo "Error: Could not find project directory"; exit 1; }

# Check if data folder exists
if [ ! -d "node-web-api/data" ]; then
    echo "⚠️  Data folder not found in node-web-api/"
    echo "Creating data folder structure..."
    
    mkdir -p node-web-api/data
    
    # Create minimal seed.ts file
    cat > node-web-api/data/seed.ts << 'SEEDEOF'
// Main seed script
import mongoose from 'mongoose';
import dotenv from 'dotenv';

dotenv.config();

// Import seed functions (create empty if not needed)
async function seedAll() {
  try {
    console.log('🌱 Starting database seeding...');
    
    // Connect to MongoDB
    const mongoUri = process.env.MONGODB_URI || 'mongodb://localhost:27017/ces-lms';
    await mongoose.connect(mongoUri);
    console.log('✅ Connected to MongoDB');
    
    // Add your seed logic here
    // Example: await seedUsers();
    // Example: await seedCategories();
    
    console.log('✅ Seeding completed');
    await mongoose.disconnect();
    process.exit(0);
  } catch (error) {
    console.error('❌ Seeding failed:', error);
    process.exit(1);
  }
}

// Run if called directly
if (require.main === module) {
  seedAll();
}

export default seedAll;
SEEDEOF

    # Create a README for the data folder
    cat > node-web-api/data/README.md << 'READMEEOF'
# Seed Data Folder

This folder contains database seed scripts.

## Files

- `seed.ts` - Main seed script entry point
- Add other seed files as needed (e.g., `users.seed.ts`, `courses.seed.ts`)

## Usage

The seed script is automatically run when `RUN_SEED=true` is set in environment variables.
READMEEOF

    echo "✅ Created data folder with minimal seed.ts"
    echo ""
    echo "📁 Created files:"
    ls -la node-web-api/data/
    echo ""
else
    echo "✅ Data folder already exists"
    echo ""
    echo "📁 Current contents:"
    ls -la node-web-api/data/
    echo ""
fi

# Verify the folder structure
echo "Verifying folder structure..."
if [ -d "node-web-api/data" ]; then
    echo "✅ Data folder exists"
    if [ -f "node-web-api/data/seed.ts" ]; then
        echo "✅ seed.ts file exists"
        FILE_SIZE=$(wc -c < node-web-api/data/seed.ts)
        echo "   File size: ${FILE_SIZE} bytes"
    else
        echo "⚠️  seed.ts file not found, but folder exists"
    fi
else
    echo "❌ Data folder still missing!"
    exit 1
fi

echo ""
echo "============================================"
echo "Fix Complete!"
echo "============================================"
echo ""
echo "🚀 You can now build:"
echo "   sudo docker compose up -d --build"
echo ""

