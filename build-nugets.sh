#!/bin/bash

# Build and pack all NuGet packages
# Usage: ./build-nugets.sh

set -e

echo "======================================"
echo "Building Tsikhanau.Packages NuGets"
echo "======================================"

# Clean previous builds
echo ""
echo "Cleaning previous builds..."
dotnet clean --configuration Release

# Build in dependency order
echo ""
echo "Building packages in dependency order..."

# 1. Foundation (no dependencies)
echo ""
echo "[1/7] Building Tsikhanau.Foundation..."
dotnet pack Tsikhanau.Foundation/Tsikhanau.Foundation.csproj --configuration Release --output ./nugets

# 2. Outcomes (depends on Foundation)
echo ""
echo "[2/7] Building Tsikhanau.Outcomes..."
dotnet pack Tsikhanau.Outcomes/Tsikhanau.Outcomes.csproj --configuration Release --output ./nugets

# 3. ValueObjects (depends on Outcomes)
echo ""
echo "[3/7] Building Tsikhanau.ValueObjects..."
dotnet pack Tsikhanau.ValueObjects/Tsikhanau.ValueObjects.csproj --configuration Release --output ./nugets

# 4. RailwayExtensions (depends on Foundation + Outcomes)
echo ""
echo "[4/7] Building Tsikhanau.RailwayExtensions..."
dotnet pack Tsikhanau.RailwayExtensions/Tsikhanau.RailwayExtensions.csproj --configuration Release --output ./nugets

# 5. Validation (depends on Foundation + Outcomes + ValueObjects)
echo ""
echo "[5/7] Building Tsikhanau.Validation..."
dotnet pack Tsikhanau.Validation/Tsikhanau.Validation.csproj --configuration Release --output ./nugets

# 6. Guards (independent)
echo ""
echo "[6/7] Building Tsikhanau.Guards..."
dotnet pack Tsikhanau.Guards/Tsikhanau.Guards.csproj --configuration Release --output ./nugets

# 7. Flow (depends on Foundation + Outcomes + RailwayExtensions)
echo ""
echo "[7/7] Building Tsikhanau.Flow..."
dotnet pack Tsikhanau.Flow/Tsikhanau.Flow.csproj --configuration Release --output ./nugets

echo ""
echo "======================================"
echo "✓ All packages built successfully!"
echo "======================================"
echo ""
echo "NuGet packages are in ./nugets/"
echo ""
ls -lh ./nugets/*.nupkg
echo ""
echo "To publish to NuGet.org, run:"
echo "  dotnet nuget push ./nugets/<package>.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json"
