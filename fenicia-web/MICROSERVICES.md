# Fenicia Web - Microservices Configuration

## Environment Variables

The application uses environment variables to configure API endpoints for different microservices.

### Files

- **`.env`** - Your local environment configuration (NOT committed to git)
- **`.env.example`** - Template file with documentation (committed to git)

### Configuration

Copy `.env.example` to `.env` and update the values for your environment:

```bash
cp .env.example .env
```

### Available Variables

| Variable | Description | Default | Example |
|----------|-------------|---------|---------|
| `VITE_AUTH_API_BASE_URL` | Authentication microservice URL | `http://localhost:5144` | `http://auth-api.example.com` |
| `VITE_BASIC_API_BASE_URL` | Basic module microservice URL | `http://localhost:5083` | `http://basic-api.example.com` |
| `VITE_API_BASE_URL` | General purpose API URL | `http://localhost:5000` | `http://api.example.com` |
| `VITE_DEFAULT_COMPANY_ID` | Default company ID for anonymous requests | `00000000-0000-0000-0000-000000000000` | UUID string |

## Microservices Architecture

### Authentication Service (`VITE_AUTH_API_BASE_URL`)
Handles all authentication and authorization operations:
- **Routes:** `/token`, `/register`, `/company`, `/forgotpassword`
- **Features:** Login, registration, password recovery, company selection, token management
- **Default:** `http://localhost:5144`

### Basic Module Service (`VITE_BASIC_API_BASE_URL`)
Handles core business operations:
- **Routes:** `/customer`, `/supplier`, `/product`, `/productcategory`, `/employee`, `/position`, `/inventory`, `/stockmovement`, `/order`
- **Features:** CRUD operations for customers, suppliers, products, employees, inventory management
- **Default:** `http://localhost:5083`

### General API (`VITE_API_BASE_URL`)
General purpose API for future use:
- **Default:** `http://localhost:5000`

## Service Clients

All API clients automatically:
- Use the configured base URLs from environment variables
- Include `Authorization: Bearer <token>` header when authenticated
- Include `x-company` header with selected company ID
- Handle 401 errors by clearing auth and redirecting to login

### Usage Example

```typescript
import { AuthTokenClient } from './services/auth-token-client';
import { BasicCustomerClient } from './services/basic-crud-clients';

// Authentication client (uses VITE_AUTH_API_BASE_URL)
const authClient = new AuthTokenClient();
await authClient.generateToken({ email, password });

// Basic module client (uses VITE_BASIC_API_BASE_URL)
const customerClient = new BasicCustomerClient();
const customers = await customerClient.getAll();
```

## Adding New Microservices

1. Add new environment variable to `.env` and `.env.example`:
   ```
   VITE_NEW_SERVICE_API_BASE_URL=http://localhost:XXXX
   ```

2. Create a new client class extending `ApiClient`:
   ```typescript
   import { ApiClient, NEW_SERVICE_API_BASE_URL } from './client';
   
   export class NewServiceClient extends ApiClient {
     constructor(baseURL: string = NEW_SERVICE_API_BASE_URL) {
       super(baseURL);
     }
     
     // Add your API methods here
   }
   ```

3. Use the client in your components:
   ```typescript
   const client = new NewServiceClient();
   const data = await client.someMethod();
   ```

## Security Notes

- ⚠️ **Never commit `.env` file** - It's excluded in `.gitignore`
- ✅ **Commit `.env.example`** - Provides template for other developers
- 🔐 **Store secrets securely** - Use environment variables for sensitive data
- 🔄 **Different environments** - Use different `.env` files for dev/staging/production
