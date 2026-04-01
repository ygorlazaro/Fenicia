export type GetAllCustomerResponse = { 
    id: string;
    personId: string;
    name: string;
    email?: string;
    phoneNumber?: string;
    document?: string;
    address: AddressResponse;
}

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
}

export type GetCustomerByIdResponse = { 
    id: string;
    personId: string;
    name: string;
    email?: string;
    phoneNumber?: string;
    document?: string;
    address: AddressResponse;
}

export type AddCustomerCommand = {
    name: string;
    email?: string;
    phoneNumber?: string;
    document?: string;
    address: AddressCommand;
}

export type AddressCommand = {
    street: string;
    number: string;
    complement?: string;
    neighborhood?: string;
    city: string;
    stateId: string;
    zipCode: string;
    country?: string;
}

export type AddCustomerResponse = { 
    id: string;
    personId: string;
}

export type UpdateCustomerCommand = {
    id: string;
    name: string;
    email?: string;
    document?: string;
    phoneNumber?: string;
    address?: AddressCommand;
}

export type UpdateCustomerResponse = { 
    id: string;
    personId: string;
}

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
}

export type GetEmployeeByIdResponse = { 
    id: string;
    positionId: string;
    personId: string;
    name: string;
    email?: string;
    phoneNumber?: string;
    document?: string;
    address?: AddressResponse;
}

export type AddEmployeeCommand = {
    id: string;
    positionId: string;
    name: string;
    email?: string;
    document?: string;
    phoneNumber?: string;
    address?: AddressCommand;
}

export type AddEmployeeResponse = { 
    id: string;
    positionId: string;
    personId: string;
}

export type UpdateEmployeeCommand = {
    id: string;
    positionId: string;
    name: string;
    email?: string;
    document?: string;
    phoneNumber?: string;
    address?: AddressCommand;
}

export type UpdateEmployeeResponse = { 
    id: string;
    positionId: string;
    personId: string;
}

export type GetAllStateResponse = { 
    id: string;
    name: string;
    uf: string;
}

export type GetAllPositionResponse = {
    id: string;
    name: string;
};

export type GetPositionByIdResponse = {
    id: string;
    name: string;
};

export type AddPositionCommand = {
    id: string;
    name: string;
};

export type AddPositionResponse = {
    id: string;
    name: string;
};

export type UpdatePositionCommand = {
    id: string;
    name: string;
};

export type UpdatePositionResponse = {
    id: string;
    name: string;
};

export type GetAllProductResponse = {
    id: string;
    name: string;
    sku?: string;
    barcode?: string;
    description?: string;
    costPrice?: number;
    salesPrice: number;
    quantity: number;
    minStockLevel?: number;
    maxStockLevel?: number;
    imageUrl?: string;
    weight?: number;
    dimensions?: string;
    unitOfMeasure?: string;
    categoryId: string;
    categoryName: string;
    supplierId?: string;
    supplierName?: string;
    isActive: boolean;
};

export type GetProductByIdResponse = GetAllProductResponse;

export type AddProductCommand = {
    id: string;
    name: string;
    sku?: string;
    barcode?: string;
    description?: string;
    costPrice?: number;
    salesPrice: number;
    quantity: number;
    minStockLevel?: number;
    maxStockLevel?: number;
    imageUrl?: string;
    weight?: number;
    dimensions?: string;
    unitOfMeasure?: string;
    categoryId: string;
    supplierId?: string;
};

export type AddProductResponse = GetAllProductResponse;

export type UpdateProductCommand = AddProductCommand;

export type UpdateProductResponse = GetAllProductResponse;

export type GetAllSupplierResponse = {
    id: string;
    personId: string;
    name: string;
    email?: string;
    phoneNumber?: string;
    document?: string;
    address?: AddressResponse;
};

export type GetSupplierByIdResponse = GetAllSupplierResponse;

export type AddSupplierCommand = {
    name: string;
    email?: string;
    phoneNumber?: string;
    document?: string;
    address: AddressCommand;
};

export type AddSupplierResponse = {
    id: string;
    personId: string;
};

export type UpdateSupplierCommand = {
    id: string;
    name: string;
    email?: string;
    phoneNumber?: string;
    document?: string;
    address?: AddressCommand;
};

export type UpdateSupplierResponse = { 
    id: string;
    personId: string;
};

export type GetAllProductCategoryResponse = {
    id: string;
    name: string;
};

export type GetProductCategoryByIdResponse = {
    id: string;
    name: string;
};

export type AddProductCategoryCommand = {
    id: string;
    name: string;
};

export type AddProductCategoryResponse = {
    id: string;
    name: string;
};

export type UpdateProductCategoryCommand = {
    id: string;
    name: string;
};

export type UpdateProductCategoryResponse = {
    id: string;
    name: string;
};

export type PaymentMethod = 'Cash' | 'CreditCard' | 'DebitCard' | 'BankTransfer' | 'Pix';

export type OrderStatus = 'Pending' | 'Confirmed' | 'Shipped' | 'Delivered' | 'Cancelled';

export type GetAllOrderResponse = {
    id: string;
    orderNumber: string;
    userId: string;
    customerId: string;
    customerName: string;
    totalAmount: number;
    discountAmount: number;
    totalQuantity: number;
    saleDate: string;
    status: OrderStatus;
    paymentMethod: PaymentMethod;
    totalItems: number;
    employeeId?: string;
    employeeName?: string;
};

export type GetOrderByIdResponse = {
    id: string;
    orderNumber: string;
    userId: string;
    customerId: string;
    customerName: string;
    totalAmount: number;
    discountAmount: number;
    totalQuantity: number;
    saleDate: string;
    status: OrderStatus;
    paymentMethod: PaymentMethod;
    notes?: string;
    details: OrderDetailResponse[];
    employeeId?: string;
};

export type CreateOrderCommand = {
    customerId: string;
    items: Array<{
        productId: string;
        quantity: number;
        price: number;
    }>;
    discountAmount?: number;
    paymentMethod: PaymentMethod;
    notes?: string;
    employeeId?: string;
};

export type CreateOrderResponse = {
    id: string;
    orderNumber: string;
    userId: string;
    customerId: string;
    totalAmount: number;
    discountAmount: number;
    totalQuantity: number;
    saleDate: string;
    status: OrderStatus;
    paymentMethod: PaymentMethod;
    notes?: string;
    employeeId?: string;
};

export type OrderDetailResponse = {
    id: string;
    orderId: string;
    productId: string;
    price: number;
    discountAmount: number;
    quantity: number;
    subtotal: number;
};

export type DataSourceItem = {
    id: string;
    name: string;
};
