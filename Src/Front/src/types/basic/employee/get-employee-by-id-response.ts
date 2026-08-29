import { AddressResponse } from "../address/address-response";

export type GetEmployeeByIdResponse = {
    id: string;
    positionId: string;
    personId: string;
    name: string;
    email?: string;
    phoneNumber?: string;
    document?: string;
    address?: AddressResponse;
};
