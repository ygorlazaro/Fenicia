import { AddressCommand } from "../address/address-command";

export type AddCustomerCommand = {
    id: string;
    name: string;
    email?: string;
    phoneNumber?: string;
    document?: string;
    address: AddressCommand;
};
