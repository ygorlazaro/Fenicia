import { AddressCommand } from "../address/address-command";

export type AddSupplierCommand = {
    name: string;
    email?: string;
    phoneNumber?: string;
    document?: string;
    address: AddressCommand;
};
