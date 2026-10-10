#!/bin/bash
# Evaluates the metric by running acceptance tests
# Output format: METRIC=<number_of_failing_tests>

cd "$(dirname "$0")/.."
TEST_OUTPUT=$($EVAL_CMD)
BUILD_EXIT_CODE=$?
METRIC=$(echo "$TEST_OUTPUT" | grep "Passed!" | sed 's/.*Passed: *//' | sed 's/, *Failed: *//' | sed 's/, *Skipped: *//' | sed 's/, *Total: *//' | sed 's/Duration:.*//')
FAILED=$(echo "$TEST_OUTPUT" | grep "Failed:" | sed 's/.*Failed: *//' | sed 's/,.*//')
echo "METRIC=$FAILED"
exit $BUILD_EXIT_CODE
