import { ModuleType, SubscriptionStatus } from "../enums/auth-enums";

export type GetCompaniesByUserResponse = {
    id: string,
    name: string,
    cnpj: string,
    role: string
};

export type GetModuleResponse = {
    id: string,
    name: string,
    type: ModuleType,
    description?: string,
    icon?: string,
    isActive: boolean,
    sortOrder: number,
    price: number,
}

export type GetUserProfileResponse = {
    id: string,
    name: string,
    email: string,
    companies: UserCompanyResponse[],
    subscriptions: UserSubscriptionResponse[]
}

export type UserCompanyResponse = {
    id: string,
    name: string,
    cnpj: string
}

export type UserSubscriptionResponse = {
    id: string,
    companyId: string,
    companyName: string,
    status: SubscriptionStatus,
    startDate: Date,
    endDate?: Date
    modules: UserModuleResponse[]
}
export type UserModuleResponse = {
    id: string,
    name: string,
    type: ModuleType
}

export type CreateNewOrderResponse = {
    orderId: string;
}

export type CreateNewUserCommand = {
    email: string;
    password: string;
    name: string;
    company: CreateNewUserCompanyCommand
}

export type CreateNewUserCompanyCommand = { 
    name: string;
    cnpj: string;
}

export type CreateNewUserResponse = { 
    id: string;
    email: string;
    name: string;
    company: CreateNewUserCompanyResponse;
}

export type CreateNewUserCompanyResponse = {
    id: string;
    name: string;
    cnpj: string;
}

export type GenerateTokenQuery = { 
    email: string;
    password: string;
}

export type TokenResponse = { 
    accessToken: string;
    refreshToken: string;
    user: UserResponse;
}

export type UserResponse = {
    id: string;
    name: string;
    email: string;
}

export type ValidateTokenQuery = {
    userId: string;
    refreshToken: string;
}
