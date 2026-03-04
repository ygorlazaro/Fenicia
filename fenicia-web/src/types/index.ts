// Global type definitions for the project

declare module '*.scss' {
  const content: Record<string, string>;
  export default content;
}

declare module '*.css' {
  const content: Record<string, string>;
  export default content;
}

declare module '*.png';
declare module '*.jpg';
declare module '*.jpeg';
declare module '*.gif';
declare module '*.svg';
declare module '*.ico';

// API Response Types
export interface Pagination<T> {
  data: T;
  total: number;
  page: number;
  perPage: number;
  pages: number;
}

// Auth Types
export interface User {
  id: string;
  email: string;
  name: string;
  companyId?: string;
}

export interface Company {
  id: string;
  name: string;
  cnpj: string;
  isDefault?: boolean;
}

export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
  user: User;
}

// Employee Types
export interface Employee {
  id: string;
  name: string;
  email: string;
  phoneNumber?: string;
  positionId: string;
  positionName?: string;
  stateId: string;
  stateName?: string;
  city?: string;
  street?: string;
  number?: string;
  neighborhood?: string;
  zipCode?: string;
  complement?: string;
  document?: string;
}

// Position Types
export interface Position {
  id: string;
  name: string;
  code?: string;
  description?: string;
}

// Product Types
export interface Product {
  id: string;
  name: string;
  sku?: string;
  description?: string;
  price?: number;
  cost?: number;
  categoryId?: string;
  categoryName?: string;
  supplierId?: string;
  supplierName?: string;
}

export interface ProductCategory {
  id: string;
  name: string;
  code?: string;
  description?: string;
}

// Customer & Supplier Types
export interface Customer {
  id: string;
  name: string;
  email?: string;
  phone?: string;
  document?: string;
  address?: string;
}

export interface Supplier {
  id: string;
  name: string;
  email?: string;
  phone?: string;
  document?: string;
  address?: string;
}

// Inventory Types
export interface Inventory {
  id?: string;
  productId?: string;
  productName?: string;
  categoryId?: string;
  categoryName?: string;
  quantity: number;
  minStock: number;
  maxStock: number;
}

// Stock Movement Types
export interface StockMovement {
  id?: string;
  productId: string;
  productName?: string;
  quantity: number;
  type: 'IN' | 'OUT' | 'TRANSFER' | 'ADJUSTMENT';
  notes?: string;
  date?: string;
}

// Order Types
export interface OrderItem {
  productId: string;
  productName?: string;
  quantity: number;
  price: number;
}

export interface Order {
  id?: string;
  userId?: string;
  items: OrderItem[];
  totalAmount?: number;
  createdAt?: string;
}

// Module Types
export interface Module {
  id: string;
  name: string;
  type: string;
  price?: number;
}

// Navigation Types
export interface NavItem {
  component?: React.ComponentType<any>;
  name: string;
  to?: string;
  href?: string;
  icon?: React.ReactNode;
  badge?: {
    color: string;
    text: string;
  };
  items?: NavItem[];
  element?: React.ComponentType<any>;
  path?: string;
  exact?: boolean;
}

// Profile Types
export interface UserProfileResponse {
  id: string;
  name: string;
  email: string;
  companies: UserCompany[];
  subscriptions: UserSubscription[];
}

export interface UserCompany {
  id: string;
  name: string;
  cnpj: string;
  isDefault: boolean;
}

export interface UserSubscription {
  id: string;
  companyId: string;
  companyName: string;
  status: string;
  startDate: string;
  endDate?: string;
  modules: SubscribedModule[];
}

export interface SubscribedModule {
  id: string;
  name: string;
  type: string;
  subscribedAt: string;
}
