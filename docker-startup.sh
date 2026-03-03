#!/bin/bash

# =============================================================================
# Fenicia Platform - Docker Management Script
# =============================================================================
# Usage: ./docker-startup.sh [command]
# Commands:
#   start       - Start all services
#   stop        - Stop all services
#   restart     - Restart all services
#   build       - Build all services
#   rebuild     - Rebuild all services (no cache)
#   logs        - View logs
#   status      - Show running services
#   clean       - Remove all containers and volumes (WARNING: deletes data!)
#   dev         - Start infrastructure only (for local development)
# =============================================================================

set -e

COMPOSE_FILE="Docker/docker-compose.yml"
COMPOSE_CMD="docker-compose -f $COMPOSE_FILE"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

check_docker() {
    if ! command -v docker &> /dev/null; then
        print_error "Docker is not installed. Please install Docker first."
        exit 1
    fi
    
    if ! docker ps &> /dev/null; then
        print_error "Docker daemon is not running. Please start Docker."
        exit 1
    fi
}

start_all() {
    print_info "Starting all Fenicia services..."
    $COMPOSE_CMD up -d --build
    print_success "All services started!"
    print_info "Frontend: http://localhost:3000"
    print_info "Auth: http://localhost:5000"
    print_info "Seq Logs: http://localhost:5341"
}

stop_all() {
    print_info "Stopping all Fenicia services..."
    $COMPOSE_CMD down
    print_success "All services stopped!"
}

restart_all() {
    print_info "Restarting all Fenicia services..."
    $COMPOSE_CMD restart
    print_success "All services restarted!"
}

build_all() {
    print_info "Building all Fenicia services..."
    $COMPOSE_CMD build
    print_success "Build completed!"
}

rebuild_all() {
    print_warning "Rebuilding all services (no cache)..."
    $COMPOSE_CMD build --no-cache
    print_success "Rebuild completed!"
}

view_logs() {
    print_info "Viewing logs (Ctrl+C to exit)..."
    $COMPOSE_CMD logs -f
}

show_status() {
    print_info "Running services:"
    $COMPOSE_CMD ps
}

clean_all() {
    print_warning "This will remove all containers and volumes!"
    read -p "Are you sure? (y/N) " confirm
    if [[ $confirm == [yY] ]]; then
        print_info "Cleaning up..."
        $COMPOSE_CMD down -v
        docker system prune -f
        print_success "Cleanup completed!"
    else
        print_info "Cleanup cancelled."
    fi
}

dev_mode() {
    print_info "Starting infrastructure services only (development mode)..."
    $COMPOSE_CMD up -d postgres redis seq
    print_success "Infrastructure started!"
    print_info "PostgreSQL: localhost:5432"
    print_info "Redis: localhost:6379"
    print_info "Seq: http://localhost:5341"
    print_warning "Remember to run backend services locally for development"
}

show_help() {
    echo "Fenicia Platform - Docker Management"
    echo ""
    echo "Usage: $0 [command]"
    echo ""
    echo "Commands:"
    echo "  start       Start all services"
    echo "  stop        Stop all services"
    echo "  restart     Restart all services"
    echo "  build       Build all services"
    echo "  rebuild     Rebuild all services (no cache)"
    echo "  logs        View logs"
    echo "  status      Show running services"
    echo "  clean       Remove all containers and volumes"
    echo "  dev         Start infrastructure only (for local development)"
    echo "  help        Show this help message"
    echo ""
}

# Main script
check_docker

case "${1:-help}" in
    start)
        start_all
        ;;
    stop)
        stop_all
        ;;
    restart)
        restart_all
        ;;
    build)
        build_all
        ;;
    rebuild)
        rebuild_all
        ;;
    logs)
        view_logs
        ;;
    status)
        show_status
        ;;
    clean)
        clean_all
        ;;
    dev)
        dev_mode
        ;;
    help|--help|-h)
        show_help
        ;;
    *)
        print_error "Unknown command: $1"
        show_help
        exit 1
        ;;
esac
