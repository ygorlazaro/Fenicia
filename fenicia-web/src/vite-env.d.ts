/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_BASE_URL: string;
  readonly VITE_AUTH_API_BASE_URL: string;
  readonly VITE_BASIC_API_BASE_URL: string;
  readonly VITE_DEFAULT_COMPANY_ID: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
