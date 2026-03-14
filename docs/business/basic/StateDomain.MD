# State Domain

The State domain manages Brazilian states (UF) for address localization in the Fenicia system.

## Overview

This domain provides functionality for:
- Retrieving all Brazilian states
- Getting state details by ID

## Business Logic

### State Entity
A Brazilian state represents a geographic region with the following characteristics:
- **Id**: Unique identifier (GUID)
- **Name**: Full state name (e.g., Sao Paulo, Rio de Janeiro)
- **UF**: Two-letter state code (e.g., SP, RJ, MG)

### State Retrieval
- States are returned ordered alphabetically by UF code
- States are used for address information in customers and suppliers

## Components

### Controllers

#### StateController
HTTP endpoint controller providing REST API for state operations.

- `GET /State` - Retrieves list of all states

### Handlers

#### GetAllStateHandler
Handles retrieval of all states. Returns a list of states ordered by UF code.

### Queries

#### GetAllStateQuery
Query for retrieving all states (no parameters required).

### Responses

#### GetAllStateResponse
Response model for a Brazilian state.
- `Id`: State ID
- `Name`: Full state name
- `UF`: Two-letter state code

## Data Model

### StateModel
Entity representing a Brazilian state in the database:
- Mapped to `auth.states` table
- Has relationships with: Addresses, Persons

## Security

- All state endpoints require authentication

## Testing

The State domain has unit tests located in `Fenicia.Module.Basic.Tests/Domains/State/`.

### Test Coverage

#### StateControllerTests
Tests the HTTP endpoint layer including:
- Retrieving all states
- WideEventContext propagation
- Controller attribute validation

**Key test scenarios:**
- Returns list of all states ordered by UF
- Controller has correct attributes (Authorize, Route, ApiController)
