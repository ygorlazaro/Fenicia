import { UserModuleResponse } from "./user-module-response";

export type UserSubscriptionResponse = {
    id: string;
    companyId: string;
    companyName: string;
    status: string;
    startDate: Date;
    endDate?: Date;
    modules: UserModuleResponse[];
};
