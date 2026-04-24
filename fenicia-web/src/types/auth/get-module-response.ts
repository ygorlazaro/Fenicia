import { ModuleType } from "../../enums/auth-enums";


export type GetModuleResponse = {
    id: string;
    name: string;
    type: ModuleType;
    description?: string;
    icon?: string;
    isActive: boolean;
    sortOrder: number;
    price: number;
};
