import { CompanyRequest } from "./CompanyRequest";

export interface SignUpRequest {
    email: string;
    password: string;
    name: string;
    company: CompanyRequest
}


