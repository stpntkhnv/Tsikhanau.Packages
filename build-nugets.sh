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
echo "[1/4] Building Tsikhanau.Foundation..."
dotnet pack Tsikhanau.Foundation/Tsikhanau.Foundation.csproj --configuration Release --output ./nugets

# 2. Monads (depends on Foundation)
echo ""
echo "[2/4] Building Tsikhanau.Monads..."
dotnet pack Tsikhanau.Monads/Tsikhanau.Monads.csproj --configuration Release --output ./nugets

# 3. RailwayExtensions (depends on Foundation + Monads)
echo ""
echo "[3/4] Building Tsikhanau.RailwayExtensions..."
dotnet pack Tsikhanau.RailwayExtensions/Tsikhanau.RailwayExtensions.csproj --configuration Release --output ./nugets

# 4. Validation (depends on Foundation + Monads)
echo ""
echo "[4/4] Building Tsikhanau.Validation..."
dotnet pack Tsikhanau.Validation/Tsikhanau.Validation.csproj --configuration Release --output ./nugets

echo ""
echo "======================================"
echo "All packages built successfully!"
echo "======================================"
echo ""
echo "NuGet packages are in ./nugets/"
echo ""
ls -lh ./nugets/*.nupkg
echo ""
echo "To publish to NuGet.org, run:"
echo "  dotnet nuget push ./nugets/<package>.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json"
