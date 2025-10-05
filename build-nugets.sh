#!/bin/bash
set -e

NUGET_FEED="$HOME/LocalNugetFeed"

rm -rf "$NUGET_FEED"/*
mkdir -p "$NUGET_FEED"

projects=(
	"Tsikhanau.Outcomes/Tsikhanau.Outcomes.csproj"
  "Tsikhanau.Foundation/Tsikhanau.Foundation.csproj"
  "Tsikhanau.Validation/Tsikhanau.Validation.csproj"
  "Tsikhanau.ValueObjects/Tsikhanau.ValueObjects.csproj"
  "Tsikhanau.RailwayExtensions/Tsikhanau.RailwayExtensions.csproj"
)

for project in "${projects[@]}"; do
  dotnet pack "$project" -c Release -o "$NUGET_FEED"
done

