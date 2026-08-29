import { UserCompanyResponse } from "./user-company-response";
import { UserSubscriptionResponse } from "./user-subscription-response";

export type GetUserProfileResponse = {
    id: string;
    name: string;
    email: string;
    companies: UserCompanyResponse[];
    subscriptions: UserSubscriptionResponse[];
};
