#!/bin/bash
# Evaluates the metric by running acceptance tests
# Output format: METRIC=<number_of_failing_tests>

cd "$(dirname "$0")/.."

# Run tests for the specific category, capture exit code
TEST_OUTPUT=$($EVAL_CMD)
BUILD_EXIT_CODE=$?

# Extract metric from output
METRIC=$(echo "$TEST_OUTPUT" | grep "METRIC=" | tail -1 | cut -d= -f2)

echo "METRIC=$METRIC"
exit $BUILD_EXIT_CODE
