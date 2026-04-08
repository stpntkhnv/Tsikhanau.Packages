#!/bin/bash

# Publish all NuGet packages to NuGet.org
# Usage: ./publish-nugets.sh

set -e

# Check if API key is set
if [ -z "$NUGET_API_KEY" ]; then
    echo "Error: NUGET_API_KEY environment variable is not set"
    echo ""
    echo "Please set it first:"
    echo "  export NUGET_API_KEY=\"your-actual-api-key\""
    echo ""
    exit 1
fi

echo "======================================"
echo "Publishing Tsikhanau.Packages to NuGet.org"
echo "======================================"

NUGET_SOURCE="https://api.nuget.org/v3/index.json"

# Publish in dependency order
echo ""
echo "[1/7] Publishing Tsikhanau.Foundation..."
dotnet nuget push ./nugets/Tsikhanau.Foundation.1.0.0.nupkg \
    --api-key $NUGET_API_KEY \
    --source $NUGET_SOURCE \
    --skip-duplicate

echo ""
echo "[2/7] Publishing Tsikhanau.Monads..."
dotnet nuget push ./nugets/Tsikhanau.Monads.2.0.0.nupkg \
    --api-key $NUGET_API_KEY \
    --source $NUGET_SOURCE \
    --skip-duplicate

echo ""
echo "[3/7] Publishing Tsikhanau.ValueObjects..."
dotnet nuget push ./nugets/Tsikhanau.ValueObjects.1.0.0.nupkg \
    --api-key $NUGET_API_KEY \
    --source $NUGET_SOURCE \
    --skip-duplicate

echo ""
echo "[4/7] Publishing Tsikhanau.RailwayExtensions..."
dotnet nuget push ./nugets/Tsikhanau.RailwayExtensions.1.0.0.nupkg \
    --api-key $NUGET_API_KEY \
    --source $NUGET_SOURCE \
    --skip-duplicate

echo ""
echo "[5/7] Publishing Tsikhanau.Validation..."
dotnet nuget push ./nugets/Tsikhanau.Validation.1.0.1.nupkg \
    --api-key $NUGET_API_KEY \
    --source $NUGET_SOURCE \
    --skip-duplicate

echo ""
echo "[6/7] Publishing Tsikhanau.Guards..."
dotnet nuget push ./nugets/Tsikhanau.Guards.1.0.0.nupkg \
    --api-key $NUGET_API_KEY \
    --source $NUGET_SOURCE \
    --skip-duplicate

echo ""
echo "[7/7] Publishing Tsikhanau.Flow..."
dotnet nuget push ./nugets/Tsikhanau.Flow.1.0.0.nupkg \
    --api-key $NUGET_API_KEY \
    --source $NUGET_SOURCE \
    --skip-duplicate

echo ""
echo "======================================"
echo "✓ All packages published successfully!"
echo "======================================"
echo ""
echo "Packages will be available at:"
echo "  https://www.nuget.org/profiles/stpntkhnv"
echo ""
echo "Note: It may take a few minutes for packages to appear in search"
