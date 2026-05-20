import { AxiosResponse } from "axios";
import type { IPagination } from "../../types";
import type { DataSourceItem } from "../../types/basic/product-category/add-product-category-command";
import type { AddProductCommand } from "../../types/basic/product/add-product-command";
import type { AddProductResponse } from "../../types/basic/product/add-product-response";
import type { GetAllProductResponse } from "../../types/basic/product/get-all-product-response";
import type { GetProductByIdResponse } from "../../types/basic/product/get-product-by-id-response";
import { ProductPerformance } from "../../types/basic/product/product-performance";
import type { UpdateProductCommand } from "../../types/basic/product/update-product-command";
import type { UpdateProductResponse } from "../../types/basic/product/update-product-response";
import { ApiClient } from "../api-client";

export const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || "http://localhost:5083";

/**
 * Basic Product Client - Handles product CRUD operations
 */
export class BasicProductClient extends ApiClient {
    constructor(baseURL: string = BASIC_API_BASE_URL) {
        super(baseURL);
    }

    async getAll(page: number = 1, perPage: number = 10): Promise<IPagination<GetAllProductResponse>> {
        const response = await this.getClient().get("/product", { params: { page, perPage } });
        return (response as AxiosResponse).data;
    }

    async getById(id: string): Promise<GetProductByIdResponse> {
        const response = await this.getClient().get(`/product/${id}`);
        return (response as AxiosResponse).data;
    }

    async create(product: AddProductCommand): Promise<AddProductResponse> {
        const response = await this.getClient().post("/product", product);
        return (response as AxiosResponse).data;
    }

    async update(id: string, product: UpdateProductCommand): Promise<UpdateProductResponse> {
        const response = await this.getClient().patch(`/product/${id}`, product);
        return (response as AxiosResponse).data;
    }

    async delete(id: string): Promise<void> {
        await this.getClient().delete(`/product/${id}`);
    }

    async getProductCategories(): Promise<DataSourceItem[]> {
        const response = await this.getClient().get("/datasource/productcategory");
        return (response as AxiosResponse).data;
    }

    async getSuppliers(): Promise<DataSourceItem[]> {
        const response = await this.getClient().get("/datasource/supplier");
        return (response as AxiosResponse).data;
    }

    async getPerformance(days?: number, topLimit?: number): Promise<ProductPerformance> {
        const params: any = {};
        if (days !== undefined) params.days = days;
        if (topLimit !== undefined) params.topLimit = topLimit;

        const response = await this.getClient().get("/product/performance", { params });
        return (response as AxiosResponse).data;
    }
}
