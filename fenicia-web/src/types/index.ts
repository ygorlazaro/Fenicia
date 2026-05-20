// Global type definitions for the project

declare module "*.scss" {
    const content: Record<string, string>;
    export default content;
}

declare module "*.css" {
    const content: Record<string, string>;
    export default content;
}

declare module "*.png";
declare module "*.jpg";
declare module "*.jpeg";
declare module "*.gif";
declare module "*.svg";
declare module "*.ico";

// API Response Types
export interface IPagination<T> {
    data: T[];
    total: number;
    page: number;
    perPage: number;
    pages: number;
}

// Auth Types
export interface IUser {
    id: string;
    email: string;
    name: string;
    companyId?: string;
}

export interface ICompany {
    id: string;
    name: string;
    cnpj: string;
    isDefault?: boolean;
}

export interface ITokenResponse {
    accessToken: string;
    refreshToken: string;
    user: IUser;
}

// Employee Types
export interface IEmployee {
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
export interface IPosition {
    id: string;
    name: string;
    code?: string;
    description?: string;
}

// Product Types
export interface IProduct {
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

export interface IProductCategory {
    id: string;
    name: string;
    code?: string;
    description?: string;
}

// Customer & Supplier Types
export interface ICustomer {
    id: string;
    name: string;
    email?: string;
    phone?: string;
    document?: string;
    address?: string;
}

export interface ISupplier {
    id: string;
    name: string;
    email?: string;
    phone?: string;
    document?: string;
    address?: string;
}

// Inventory Types
export interface IInventory {
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
export interface IStockMovement {
    id?: string;
    productId: string;
    productName?: string;
    quantity: number;
    type: "IN" | "OUT" | "TRANSFER" | "ADJUSTMENT";
    notes?: string;
    date?: string;
}

// Order Types
export interface IOrderItem {
    productId: string;
    productName?: string;
    quantity: number;
    price: number;
}

export interface IOrder {
    id?: string;
    userId?: string;
    items: IOrderItem[];
    totalAmount?: number;
    createdAt?: string;
}

// Module Types
export interface IModule {
    id: string;
    name: string;
    type: string;
    price?: number;
}

// Navigation Types
export interface INavItem {
    component?: React.ComponentType<any>;
    name: string;
    to?: string;
    href?: string;
    icon?: React.ReactNode;
    badge?: {
        color: string;
        text: string;
    };
    items?: INavItem[];
    element?: React.ComponentType<any>;
    path?: string;
    exact?: boolean;
}

// Profile Types
export interface IUserProfileResponse {
    id: string;
    name: string;
    email: string;
    companies: IUserCompany[];
    subscriptions: IUserSubscription[];
}

export interface IUserCompany {
    id: string;
    name: string;
    cnpj: string;
    isDefault: boolean;
}

export interface IUserSubscription {
    id: string;
    companyId: string;
    companyName: string;
    status: string;
    startDate: string;
    endDate?: string;
    modules: ISubscribedModule[];
}

export interface ISubscribedModule {
    id: string;
    name: string;
    type: string;
    subscribedAt: string;
}
