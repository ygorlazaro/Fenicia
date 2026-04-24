import { AddressCommand } from "../address/address-command";


export type UpdateEmployeeCommand = {
    id: string;
    positionId: string;
    name: string;
    email?: string;
    document?: string;
    phoneNumber?: string;
    address?: AddressCommand;
};
