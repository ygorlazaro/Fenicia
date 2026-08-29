import { CreateNewUserCompanyResponse } from "./create-new-user-company-response";

export type CreateNewUserResponse = {
    id: string;
    email: string;
    name: string;
    company: CreateNewUserCompanyResponse;
};
