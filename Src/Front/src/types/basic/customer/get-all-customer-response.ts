import { AddressResponse } from "../address/address-response";

export type GetAllCustomerResponse = {
    id: string;
    personId: string;
    name: string;
    email?: string;
    phoneNumber?: string;
    document?: string;
    address: AddressResponse;
};
