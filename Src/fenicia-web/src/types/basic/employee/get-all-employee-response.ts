import { AddressResponse } from "../address/address-response";

export type GetAllEmployeeResponse = {
    id: string;
    positionId: string;
    personId: string;
    name: string;
    email?: string;
    phoneNumber?: string;
    document?: string;
    positionName?: string;
    address?: AddressResponse;
};
