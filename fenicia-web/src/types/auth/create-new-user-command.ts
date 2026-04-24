import { CreateNewUserCompanyCommand } from "./create-new-user-company-command";

export type CreateNewUserCommand = {
    email: string;
    password: string;
    name: string;
    company: CreateNewUserCompanyCommand;
};
