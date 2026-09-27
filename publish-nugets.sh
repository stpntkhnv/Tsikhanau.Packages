#!/bin/bash

# Pack Tsikhanau.Railway and publish it to NuGet.org
# Usage: ./publish-nugets.sh

set -e

if [ -z "$NUGET_API_KEY" ]; then
    echo "Error: NUGET_API_KEY environment variable is not set"
    echo ""
    echo "Please set it first:"
    echo "  export NUGET_API_KEY=\"your-actual-api-key\""
    echo ""
    exit 1
fi

PROJECT="Tsikhanau.Railway/Tsikhanau.Railway.csproj"
NUGET_SOURCE="https://api.nuget.org/v3/index.json"
VERSION=$(dotnet msbuild "$PROJECT" -getProperty:Version)
PACKAGE="./nugets/Tsikhanau.Railway.$VERSION.nupkg"

echo "======================================"
echo "Publishing Tsikhanau.Railway $VERSION to NuGet.org"
echo "======================================"

echo ""
echo "Packing..."
dotnet pack "$PROJECT" --configuration Release --output ./nugets

echo ""
echo "Pushing $PACKAGE..."
dotnet nuget push "$PACKAGE" \
    --api-key "$NUGET_API_KEY" \
    --source "$NUGET_SOURCE" \
    --skip-duplicate

echo ""
echo "======================================"
echo "Package published successfully!"
echo "======================================"
echo ""
echo "Package will be available at:"
echo "  https://www.nuget.org/packages/Tsikhanau.Railway"
echo ""
echo "Note: It may take a few minutes for the package to appear in search"
