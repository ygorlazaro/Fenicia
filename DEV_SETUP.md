# Development Setup

## Prerequisites

Start infrastructure services with Docker:

```bash
docker compose -f Docker/docker-compose.yml up postgres redis seq
```

Infrastructure stays in Docker. Only .NET services and the frontend run natively for fast iteration.

## Running the Application

### VS Code (Recommended)

Press **F5** and select **Full Stack** to launch all backend modules and the frontend simultaneously with hot reload.

### Command Line

```bash
./start-dev.sh
```

Press **Ctrl+C** to stop all services cleanly.

## Port Map

| Service | Port |
|---------|------|
| fenicia-auth | 5000 |
| fenicia-module-basic | 5083 |
| fenicia-module-projects | 5144 |
| fenicia-module-accounting | 5010 |
| fenicia-module-contracts | 5012 |
| fenicia-module-customersupport | 5014 |
| fenicia-module-ecommerce | 5016 |
| fenicia-module-hr | 5018 |
| fenicia-module-performanceevaluation | 5020 |
| fenicia-module-pos | 5022 |
| fenicia-module-plus | 5024 |
| fenicia-module-socialnetwork | 5026 |
| fenicia-web (frontend, Vite dev) | 5173 |

## Frontend Environment Variables

The frontend reads backend URLs from environment variables in `Src/Front/.env.local`:

```
VITE_AUTH_API_BASE_URL=http://localhost:5000/api
VITE_BASIC_API_BASE_URL=http://localhost:5083/api
VITE_PROJECTS_API_BASE_URL=http://localhost:5144/api
VITE_ACCOUNTING_API_BASE_URL=http://localhost:5010/api
VITE_CONTRACTS_API_BASE_URL=http://localhost:5012/api
VITE_CUSTOMER_SUPPORT_API_BASE_URL=http://localhost:5014/api
VITE_ECOMMERCE_API_BASE_URL=http://localhost:5016/api
VITE_HR_API_BASE_URL=http://localhost:5018/api
VITE_PERFORMANCE_EVALUATION_API_BASE_URL=http://localhost:5020/api
VITE_POS_API_BASE_URL=http://localhost:5022/api
VITE_PLUS_API_BASE_URL=http://localhost:5024/api
VITE_SOCIAL_NETWORK_API_BASE_URL=http://localhost:5026/api
```

See `Src/Front/.env.example` for the full list.
