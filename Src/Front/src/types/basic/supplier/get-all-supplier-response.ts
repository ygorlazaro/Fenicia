import { AddressResponse } from "../address/address-response";

export type GetAllSupplierResponse = {
    id: string;
    personId: string;
    name: string;
    email?: string;
    phoneNumber?: string;
    document?: string;
    address?: AddressResponse;
};
