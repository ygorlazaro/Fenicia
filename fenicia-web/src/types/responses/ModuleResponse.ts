export interface ModuleResponse { 
    id: number,
    name: string,
    amount: number,
    type: ModuleType
}

export enum ModuleType {
    Erp = -1,
    Auth = 0,
    Basic = 1,
    SocialNetwork = 2,
    Project = 3,
    PerformanceEvaluation = 4,
    Accounting = 5,
    Hr = 6,
    Pos = 7,
    Contracts = 8,
    Ecommerce = 9,
    CustomerSupport = 10,
    Plus = 11
}
