#!/bin/bash

# Build and pack Tsikhanau.Railway
# Usage: ./build-nugets.sh

set -e

echo "======================================"
echo "Building Tsikhanau.Railway NuGet"
echo "======================================"

echo ""
echo "Cleaning previous builds..."
dotnet clean --configuration Release

echo ""
echo "Packing Tsikhanau.Railway..."
dotnet pack Tsikhanau.Railway/Tsikhanau.Railway.csproj --configuration Release --output ./nugets

echo ""
echo "======================================"
echo "Package built successfully!"
echo "======================================"
echo ""
ls -lh ./nugets/*.nupkg
echo ""
echo "To publish to NuGet.org, run:"
echo "  ./publish-nugets.sh"
