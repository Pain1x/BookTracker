#!/bin/bash
# Evaluates the metric by compiling and counting compiler warnings
# Output format: METRIC=<number>

cd "$(dirname "$0")/.."

# Clean and build, capturing warnings
BUILD_OUTPUT=$(dotnet build BlazorWebApp/BlazorWebApp.csproj -v q 2>&1)
BUILD_EXIT_CODE=$?

# Count warning lines (CS0... or CS8...)
WARNING_COUNT=$(echo "$BUILD_OUTPUT" | grep -E "^\s*(warning|CS[0-9]+)" | wc -l)

echo "METRIC=$WARNING_COUNT"
exit $BUILD_EXIT_CODE
