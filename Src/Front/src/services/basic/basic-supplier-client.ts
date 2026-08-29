import { AxiosResponse } from "axios";
import { IPagination } from "../../types/index.ts";
import { AddSupplierCommand } from "../../types/basic/supplier/add-supplier-command";
import { AddSupplierResponse } from "../../types/basic/supplier/add-supplier-response";
import { GetAllSupplierResponse } from "../../types/basic/supplier/get-all-supplier-response";
import { GetSupplierByIdResponse } from "../../types/basic/supplier/get-supplier-by-id-response";
import { SupplierPerformance } from "../../types/basic/supplier/supplier-performance";
import { UpdateSupplierCommand } from "../../types/basic/supplier/update-supplier-command";
import { UpdateSupplierResponse } from "../../types/basic/supplier/update-supplier-response";
import { ApiClient } from "../api-client.ts";
import { BASIC_API_BASE_URL } from "./basic-product-client";

/**
 * Basic Supplier Client - Handles supplier CRUD operations
 */

export class BasicSupplierClient extends ApiClient {
    constructor(baseURL: string = BASIC_API_BASE_URL) {
        super(baseURL);
    }

    async getAll(page: number = 1, perPage: number = 10): Promise<IPagination<GetAllSupplierResponse>> {
        const response = await this.getClient().get("/supplier", { params: { page, perPage } });
        return (response as AxiosResponse).data;
    }

    async getById(id: string): Promise<GetSupplierByIdResponse> {
        const response = await this.getClient().get(`/supplier/${id}`);
        return (response as AxiosResponse).data;
    }

    async create(supplier: AddSupplierCommand): Promise<AddSupplierResponse> {
        const response = await this.getClient().post("/supplier", supplier);
        return (response as AxiosResponse).data;
    }

    async update(id: string, supplier: UpdateSupplierCommand): Promise<UpdateSupplierResponse> {
        const response = await this.getClient().patch(`/supplier/${id}`, supplier);
        return (response as AxiosResponse).data;
    }

    async delete(id: string): Promise<void> {
        await this.getClient().delete(`/supplier/${id}`);
    }

    async getPerformance(days?: number, topLimit?: number): Promise<SupplierPerformance> {
        const params: any = {};
        if (days !== undefined) params.days = days;
        if (topLimit !== undefined) params.topLimit = topLimit;

        const response = await this.getClient().get("/supplier/performance", { params });
        return (response as AxiosResponse).data;
    }
}
