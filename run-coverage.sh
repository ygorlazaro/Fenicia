#!/bin/bash

# =============================================================================
# Code Coverage Script for Fenicia.Module.Basic
# =============================================================================
# Usage: ./run-coverage.sh [options]
# Options:
#   --html    Generate HTML report (requires reportgenerator)
#   --clean   Clean previous test results
#   --help    Show this help message
# =============================================================================

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

TEST_PROJECT="Fenicia.Module.Basic.Tests/Fenicia.Module.Basic.Tests.csproj"
RESULTS_DIR="./TestResults"
COVERAGE_DIR="./TestResults/coverage"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

print_info() { echo -e "${BLUE}ℹ️  $1${NC}"; }
print_success() { echo -e "${GREEN}✅ $1${NC}"; }
print_warning() { echo -e "${YELLOW}⚠️  $1${NC}"; }
print_error() { echo -e "${RED}❌ $1${NC}"; }

show_help() {
    echo "Fenicia.Module.Basic - Code Coverage Script"
    echo ""
    echo "Usage: $0 [options]"
    echo ""
    echo "Options:"
    echo "  --html    Generate HTML report (requires reportgenerator tool)"
    echo "  --clean   Clean previous test results before running"
    echo "  --help    Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0                    # Run tests with coverage"
    echo "  $0 --clean --html     # Clean, run tests, generate HTML report"
    echo ""
}

# Parse arguments
GENERATE_HTML=false
CLEAN_RESULTS=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --html)
            GENERATE_HTML=true
            shift
            ;;
        --clean)
            CLEAN_RESULTS=true
            shift
            ;;
        --help)
            show_help
            exit 0
            ;;
        *)
            print_error "Unknown option: $1"
            show_help
            exit 1
            ;;
    esac
done

# Check if dotnet is available
if ! command -v dotnet &> /dev/null; then
    print_error "dotnet CLI is not installed or not in PATH"
    echo ""
    echo "Please install .NET SDK 10.0 from: https://dotnet.microsoft.com/download"
    echo "Or add it to your PATH if it's installed in a custom location"
    exit 1
fi

print_info "Using dotnet version: $(dotnet --version)"
echo ""

# Clean previous results if requested
if [ "$CLEAN_RESULTS" = true ]; then
    print_info "Cleaning previous test results..."
    rm -rf "$RESULTS_DIR"
    rm -rf "Fenicia.Module.Basic.Tests/bin"
    rm -rf "Fenicia.Module.Basic.Tests/obj"
    print_success "Cleaned!"
    echo ""
fi

# Create results directory
mkdir -p "$RESULTS_DIR"

# Run tests with code coverage
print_info "Running tests with code coverage..."
echo ""

dotnet test "$TEST_PROJECT" \
    --collect:"XPlat Code Coverage" \
    --results-directory "$RESULTS_DIR" \
    --verbosity normal \
    --logger "console;verbosity=detailed" \
    -- \
    DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

echo ""
print_success "Tests completed!"
echo ""

# Find the coverage result file
COVERAGE_FILE=$(find "$RESULTS_DIR" -name "coverage.cobertura.xml" -o -name "coverage.opencover.xml" -o -name "coverage.json" 2>/dev/null | head -1)

if [ -z "$COVERAGE_FILE" ]; then
    COVERAGE_FILE=$(find "$RESULTS_DIR" -name "*.xml" -path "*/coverage/*" 2>/dev/null | head -1)
fi

if [ -z "$COVERAGE_FILE" ]; then
    print_warning "Could not find coverage report file"
    echo "Check the $RESULTS_DIR directory manually"
    exit 1
fi

print_info "Coverage report generated: $COVERAGE_FILE"
echo ""

# Show summary if available
print_info "Coverage Results Location:"
echo "  - XML Report: $COVERAGE_FILE"
echo "  - Results Directory: $RESULTS_DIR"
echo ""

# Generate HTML report if requested
if [ "$GENERATE_HTML" = true ]; then
    print_info "Generating HTML report..."
    
    if ! command -v dotnet-reportgenerator-globaltool &> /dev/null; then
        print_warning "ReportGenerator tool not found. Installing..."
        dotnet tool install -g dotnet-reportgenerator-globaltool || {
            print_error "Failed to install ReportGenerator"
            echo "You can install it manually: dotnet tool install -g dotnet-reportgenerator-globaltool"
            exit 1
        }
    fi
    
    mkdir -p "$COVERAGE_DIR"
    
    # Detect format
    if [[ "$COVERAGE_FILE" == *"opencover"* ]]; then
        REPORTTYPE="OpenCover"
    elif [[ "$COVERAGE_FILE" == *"cobertura"* ]]; then
        REPORTTYPE="Cobertura"
    else
        REPORTTYPE="Auto"
    fi
    
    dotnet reportgenerator \
        -reports:"$COVERAGE_FILE" \
        -targetdir:"$COVERAGE_DIR" \
        -reporttypes:"HtmlSummary" \
        -verbosity:Warning
    
    print_success "HTML report generated: $COVERAGE_DIR/index.html"
    echo ""
fi

# Print test summary
print_info "Test Summary:"
dotnet test "$TEST_PROJECT" --no-build --verbosity quiet 2>&1 | grep -E "(Passed|Failed|Total)" || true

echo ""
print_success "Done! 🎉"
