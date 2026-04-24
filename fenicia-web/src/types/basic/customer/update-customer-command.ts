import { AddressCommand } from "../address/address-command";


export type UpdateCustomerCommand = {
    id: string;
    name: string;
    email?: string;
    document?: string;
    phoneNumber?: string;
    address?: AddressCommand;
};
