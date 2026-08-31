/// <reference types="vite/client" />

interface ImportMetaEnv {
    readonly VITE_API_BASE_URL: string;
    readonly VITE_AUTH_API_BASE_URL: string;
    readonly VITE_BASIC_API_BASE_URL: string;
    readonly VITE_PROJECTS_API_BASE_URL: string;
    readonly VITE_ACCOUNTING_API_BASE_URL: string;
    readonly VITE_CONTRACTS_API_BASE_URL: string;
    readonly VITE_CUSTOMER_SUPPORT_API_BASE_URL: string;
    readonly VITE_ECOMMERCE_API_BASE_URL: string;
    readonly VITE_HR_API_BASE_URL: string;
    readonly VITE_PERFORMANCE_EVALUATION_API_BASE_URL: string;
    readonly VITE_POS_API_BASE_URL: string;
    readonly VITE_PLUS_API_BASE_URL: string;
    readonly VITE_SOCIAL_NETWORK_API_BASE_URL: string;
    readonly VITE_DEFAULT_COMPANY_ID: string;
}

interface ImportMeta {
    readonly env: ImportMetaEnv;
}
