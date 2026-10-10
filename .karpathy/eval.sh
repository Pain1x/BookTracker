#!/bin/bash
# Evaluates the metric by running acceptance tests
# Output format: METRIC=<number_of_failing_tests>

cd "$(dirname "$0")/.."
source ./.karpathy/config.env

# Run the guard command and capture output
TEST_OUTPUT=$($GUARD_CMD 2>&1)
BUILD_EXIT_CODE=$?

# Extract failed count from "Passed!  - Failed: X, Passed: Y, Skipped: Z, Total: W"
METRIC=$(echo "$TEST_OUTPUT" | grep -oP 'Failed: \K\d+' || echo "0")

echo "METRIC=$METRIC"
exit $BUILD_EXIT_CODE
