export type AddressResponse = {
    id: string;
    street: string;
    number: string;
    complement?: string;
    neighborhood?: string;
    city: string;
    stateId: string;
    stateName: string;
    zipCode: string;
    country?: string;
};
